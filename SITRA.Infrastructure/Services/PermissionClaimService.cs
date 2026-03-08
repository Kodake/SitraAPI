using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services
{
    public class PermissionClaimService : IPermissionClaimService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionClaimService> _logger;

        private const string PermissionClaimType = "Permission";

        public PermissionClaimService(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            ILogger<PermissionClaimService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task AddPermissionClaimsFromRolesAsync(User user, IEnumerable<Guid> roleIds, CancellationToken cancellationToken)
        {
            var permissionKeys = new List<string>();
            foreach (var roleId in roleIds)
            {
                var rolePermissions = await _unitOfWork.RolePermissionRepository
                    .GetPermissionsByRoleIdAsync(roleId, cancellationToken);

                permissionKeys.AddRange(rolePermissions.Select(p => p.Key));
            }

            var permissionClaims = permissionKeys
                .Distinct()
                .Select(pk => new Claim(PermissionClaimType, pk))
                .ToList();

            if (permissionClaims.Any())
            {
                var result = await _userManager.AddClaimsAsync(user, permissionClaims);
                if (!result.Succeeded)
                {
                    _logger.LogError(Core.Helpers.Utilities.FailedToUpdateUserClaimsMessage(
                        string.Join(", ", result.Errors.Select(e => e.Description))));
                }
            }
        }
    }
}
