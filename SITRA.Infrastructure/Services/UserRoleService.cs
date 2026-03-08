using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly HttpContext _httpContext;
        private readonly IPermissionClaimService _permissionClaimService;
        private readonly ILogger<UserRoleService> _logger;

        private const string PermissionClaimType = "Permission";

        public UserRoleService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            IPermissionClaimService permissionClaimService,
            ILogger<UserRoleService> logger)
        {
            _context = context;
            _userManager = userManager;
            _httpContext = httpContextAccessor.HttpContext!;
            _permissionClaimService = permissionClaimService;
            _logger = logger;
        }

        public async Task<IdentityResult> UpdateUserRoleAsync(string userId, UpdateUserRoleDTO dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Add new role
            var role = await _context.Set<Role>().FindAsync([dto.RoleId], cancellationToken);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            // Update TenantRoleUsers
            var existingAssociation = await _context.TenantRoleUsers
                .FirstOrDefaultAsync(tru => tru.UserId == user.Id, cancellationToken);

            if (existingAssociation != null)
            {
                _context.TenantRoleUsers.Remove(existingAssociation);
            }

            _context.TenantRoleUsers.Add(new TenantRoleUser
            {
                RoleId = role.Id,
                UserId = user.Id
            });

            await _context.SaveChangesAsync(cancellationToken);

            // Re-assign claims — clear old permission claims first
            var currentClaims = await _userManager.GetClaimsAsync(user);
            var permissionClaims = currentClaims.Where(c => c.Type == PermissionClaimType).ToList();
            await _userManager.RemoveClaimsAsync(user, permissionClaims);

            await _permissionClaimService.AddPermissionClaimsFromRolesAsync(user, new[] { role.Id }, cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<bool> ChangeLanguageAsync(Core.Enums.Language language, CancellationToken cancellationToken)
        {
            var userId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.Language = language;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
