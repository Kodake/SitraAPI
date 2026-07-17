using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services.Auths
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserRoleService> _logger;
        private readonly IDgiiStorageService _dgiiStorageService;
        private readonly IDataProtector _protector;
        private readonly IAuthenticationService _authenticationService;

        private HttpContext HttpContext => _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext is not available.");

        public UserRoleService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserRoleService> logger,
            IDgiiStorageService dgiiStorageService,
            IDataProtectionProvider dataProtectionProvider,
            IAuthenticationService authenticationService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dgiiStorageService = dgiiStorageService;
            _protector = dataProtectionProvider.CreateProtector("CertificatePassword");
            _authenticationService = authenticationService;
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

            if (dto.CompanyLogo != null)
            {
                string rnc = !string.IsNullOrEmpty(user.RucNumber) ? user.RucNumber : "unknown";
                user.CompanyLogoUrl = await _dgiiStorageService.SaveCompanyLogoAsync(dto.CompanyLogo, rnc);
            }

            if (!string.IsNullOrEmpty(dto.CertificatePassword))
            {
                user.CertificatePassword = _protector.Protect(dto.CertificatePassword);
            }

            if (!string.IsNullOrEmpty(user.CertificateUrl) && !string.IsNullOrEmpty(user.CertificatePassword))
            {
                if (!user.IsElectronicTransmitter)
                {
                    user.IsElectronicTransmitter = true;

                    // Seed electronic fiscal supports (E-types) if they don't already exist for this tenant
                    var hasElectronic = await _context.Set<FiscalSupport>()
                        .AnyAsync(fs => fs.TenantId == user.TenantId && (int)fs.FiscalSupportType >= 31, cancellationToken);

                    if (!hasElectronic)
                    {
                        foreach (var ecfType in Enum.GetValues<SITRA.Core.Enums.EcfType>())
                        {
                            if (!SITRA.Core.Helpers.Utilities.IsElectronicType(ecfType))
                            {
                                continue;
                            }

                            var fiscalSupport = new FiscalSupport
                            {
                                Id = Guid.NewGuid(),
                                TenantId = user.TenantId,
                                FiscalSupportType = ecfType,
                                Name = ecfType switch
                                {
                                    SITRA.Core.Enums.EcfType.E31 => "Factura de Crédito Fiscal Electrónica",
                                    SITRA.Core.Enums.EcfType.E32 => "Factura de Consumo Electrónica",
                                    SITRA.Core.Enums.EcfType.E33 => "Nota de Débito Electrónica",
                                    SITRA.Core.Enums.EcfType.E34 => "Nota de Crédito Electrónica",
                                    SITRA.Core.Enums.EcfType.E41 => "Comprobante Electrónico de Compras",
                                    SITRA.Core.Enums.EcfType.E43 => "Comprobante Electrónico para Gastos Menores",
                                    SITRA.Core.Enums.EcfType.E44 => "Comprobante Electrónico para Regímenes Especiales",
                                    SITRA.Core.Enums.EcfType.E45 => "Comprobante Electrónico Gubernamental",
                                    SITRA.Core.Enums.EcfType.E46 => "Comprobante Electrónico para Exportaciones",
                                    SITRA.Core.Enums.EcfType.E47 => "Comprobante Electrónico para Pagos al Exterior",
                                    _ => Enum.GetName(ecfType) ?? string.Empty
                                },
                                InitialSequence = 0,
                                FinalSequence = ecfType == SITRA.Core.Enums.EcfType.E32 ? 50000000 : 10000000,
                                IsActive = true,
                                IsDefault = false
                            };

                            fiscalSupport.InitializeDefaults();
                            _context.Set<FiscalSupport>().Add(fiscalSupport);
                        }

                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<(byte[] CertBytes, string CertPassword)> GetTenantCertificateAsync(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new InvalidOperationException(SITRA.Core.Helpers.Utilities.NoAuthenticatedUserMessage);

            // Walk up to the owner: if this user is a sub-user, use their ParentUserId
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException(SITRA.Core.Helpers.Utilities.UserNotFoundMessage);

            var ownerUser = user.IsOwner
                ? user
                : (user.ParentUserId is not null
                    ? await _userManager.FindByIdAsync(user.ParentUserId)
                    : null)
                ?? user;

            if (string.IsNullOrEmpty(ownerUser.CertificateUrl))
                throw new InvalidOperationException(SITRA.Core.Helpers.Utilities.NoCertificateUrlConfiguredMessage);

            if (string.IsNullOrEmpty(ownerUser.CertificatePassword))
                throw new InvalidOperationException(SITRA.Core.Helpers.Utilities.NoCertificatePasswordConfiguredMessage);

            var certBytes = await _dgiiStorageService.ReadCertificateFromUrlAsync(ownerUser.CertificateUrl);
            var certPassword = _protector.Unprotect(ownerUser.CertificatePassword);

            return (certBytes, certPassword);
        }

        public async Task<bool> IsElectronicTransmitterAsync(CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // If sub-user, check the owner's flag
            var ownerUser = user.IsOwner
                ? user
                : (user.ParentUserId is not null
                    ? await _userManager.FindByIdAsync(user.ParentUserId)
                    : null)
                ?? user;

            return ownerUser.IsElectronicTransmitter;
        }
    }
}
