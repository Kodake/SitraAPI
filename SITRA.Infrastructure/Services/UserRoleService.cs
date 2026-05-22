using Microsoft.AspNetCore.DataProtection;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserRoleService> _logger;
        private readonly IDgiiStorageService _dgiiStorageService;
        private readonly IDataProtector _protector;

        private HttpContext HttpContext => _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        public UserRoleService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserRoleService> logger,
            IDgiiStorageService dgiiStorageService,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dgiiStorageService = dgiiStorageService;
            _protector = dataProtectionProvider.CreateProtector("CertificatePassword");
        }

        public async Task<IdentityResult> UpdateUserRoleAsync(string userId, UpdateUserRoleDTO dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Find new role
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

            return IdentityResult.Success;
        }

        public async Task<bool> ChangeLanguageAsync(Core.Enums.Language language, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.Language = language;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateProfileNameAsync(UpdateProfileNameDTO dto, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.UserName = dto.UserName;

            if (!string.IsNullOrEmpty(dto.Province)) user.Province = dto.Province;
            if (!string.IsNullOrEmpty(dto.Municipality)) user.Municipality = dto.Municipality;
            if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;

            if (dto.Certificate != null)
            {
                string rnc = !string.IsNullOrEmpty(user.RucNumber) ? user.RucNumber : "unknown";
                user.CertificateUrl = await _dgiiStorageService.SaveCertificateAsync(dto.Certificate, rnc);
            }

            if (!string.IsNullOrEmpty(dto.CertificatePassword))
            {
                user.CertificatePassword = _protector.Protect(dto.CertificatePassword);
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<(byte[] CertBytes, string CertPassword)> GetTenantCertificateAsync(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException("No authenticated user.");

            // Walk up to the owner: if this user is a sub-user, use their ParentUserId
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            var ownerUser = user.IsOwner
                ? user
                : (user.ParentUserId is not null
                    ? await _userManager.FindByIdAsync(user.ParentUserId)
                    : null)
                ?? user;

            if (string.IsNullOrEmpty(ownerUser.CertificateUrl))
                throw new InvalidOperationException("No certificate URL is configured for this tenant. Please upload a certificate in your profile.");

            if (string.IsNullOrEmpty(ownerUser.CertificatePassword))
                throw new InvalidOperationException("No certificate password is configured for this tenant. Please set a certificate password in your profile.");

            var certBytes = await _dgiiStorageService.ReadCertificateFromUrlAsync(ownerUser.CertificateUrl);
            var certPassword = _protector.Unprotect(ownerUser.CertificatePassword);

            return (certBytes, certPassword);
        }
    }
}
