using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Auths
{
    public interface IUserRoleService
    {
        Task<IdentityResult> UpdateUserRoleAsync(string userId, UpdateUserRoleDTO dto, CancellationToken cancellationToken);
        Task<bool> ChangeLanguageAsync(Language language, CancellationToken cancellationToken);
        Task<bool> UpdateProfileNameAsync(UpdateProfileNameDTO dto, CancellationToken cancellationToken);
        /// <summary>Returns the raw certificate bytes and plain-text password for the current tenant owner.</summary>
        Task<(byte[] CertBytes, string CertPassword)> GetTenantCertificateAsync(CancellationToken cancellationToken);
        /// <summary>Returns true if the current user (or their owner) is a certified electronic transmitter.</summary>
        Task<bool> IsElectronicTransmitterAsync(CancellationToken cancellationToken);
    }
}
