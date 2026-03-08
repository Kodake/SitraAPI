using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
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

        public async Task AddPermissionClaimsFromRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User {UserId} not found when adding permission claims.", userId);
                return;
            }

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
                    _logger.LogError(Utilities.FailedToUpdateUserClaimsMessage(
                        string.Join(", ", result.Errors.Select(e => e.Description))));
                }
            }
        }

        public async Task SyncPermissionClaimsAsync(string userId, IEnumerable<Guid> newRoleIds, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User {UserId} not found when syncing permission claims.", userId);
                return;
            }

            // Remove existing permission claims
            var currentClaims = await _userManager.GetClaimsAsync(user);
            var oldPermissionClaims = currentClaims.Where(c => c.Type == PermissionClaimType).ToList();

            if (oldPermissionClaims.Any())
            {
                await _userManager.RemoveClaimsAsync(user, oldPermissionClaims);
            }

            // Add new permission claims from roles
            await AddPermissionClaimsFromRolesAsync(userId, newRoleIds, cancellationToken);
        }
    }
}
