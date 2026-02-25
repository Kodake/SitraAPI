using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Roles;
using Microsoft.Extensions.Configuration;
using SITRA.Core.DTOs.AdministrativeTools;
using System.Security.Claims;

namespace SITRA.Core.Mediator.Handlers.AdministrativeTools.Roles
{
    public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, bool>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AssignPermissionsCommandHandler(RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<bool> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the custom role in the business repository
            var customRole = await _unitOfWork.RoleRepository.GetByIdAsync(request.DTO.RoleId, cancellationToken);
            if (customRole == null) return false;

            // 2. Find the corresponding Identity Role by name
            var identityRole = await _roleManager.FindByNameAsync(customRole.Name);
            
            if (identityRole == null)
            {
                identityRole = new IdentityRole(customRole.Name);
                var createResult = await _roleManager.CreateAsync(identityRole);
                if (!createResult.Succeeded) return false;
            }

            // 3. Load Permission Map from appsettings.json
            var config = _configuration.GetSection("PermissionManagement").Get<PermissionManagementConfigDTO>();
            var permissionMap = new Dictionary<Guid, string>();
            if (config != null)
            {
                foreach (var module in config.Modules)
                {
                    foreach (var action in module.Actions)
                    {
                        permissionMap[action.Id] = $"{module.Key}:{action.Name}";
                    }
                }
            }

            // 4. Manage Permission Claims
            var currentClaims = await _roleManager.GetClaimsAsync(identityRole);
            
            // Remove existing permission claims
            foreach (var claim in currentClaims.Where(c => c.Type == "Permission"))
            {
                await _roleManager.RemoveClaimAsync(identityRole, claim);
            }

            // Add new permission claims
            foreach (var permissionId in request.DTO.PermissionIds)
            {
                if (permissionMap.TryGetValue(permissionId, out var permissionString))
                {
                    await _roleManager.AddClaimAsync(identityRole, new Claim("Permission", permissionString));
                }
            }

            return true;
        }
    }
}
