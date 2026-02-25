using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Mediator.Commands.Auths;
using Microsoft.Extensions.Configuration;
using SITRA.Core.DTOs.AdministrativeTools;
using System.Security.Claims;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SITRA.Core.Interfaces.IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public RegisterUserCommandHandler(UserManager<User> userManager, IMapper mapper, SITRA.Core.Interfaces.IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request.Dto);

            // Create and Save Tenant
            var tenant = new SITRA.Core.Entities.Tenants.Tenant
            {
                TenantId = Guid.NewGuid(),
                Name = request.Dto.CompanyName,
                Plan = request.Dto.Plan
            };

            await _unitOfWork.Repository<SITRA.Core.Entities.Tenants.Tenant>().CreateAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            user.TenantId = tenant.TenantId;

            var result = await _userManager.CreateAsync(user, request.Dto.Password);

            if (result.Succeeded)
            {
                // Assign all permissions for all modules (Full Access)
                var config = _configuration.GetSection("PermissionManagement").Get<PermissionManagementConfigDTO>();
                if (config != null)
                {
                    foreach (var module in config.Modules)
                    {
                        foreach (var action in module.Actions)
                        {
                            await _userManager.AddClaimAsync(user, new Claim("Permission", $"{module.Key}:{action.Name}"));
                        }
                    }
                }
                // Also add wildcard for full access
                await _userManager.AddClaimAsync(user, new Claim("Permission", "*:*"));
            }

            return result;
        }
    }
}
