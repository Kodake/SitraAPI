using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserRegistrationService
    {
        Task<IdentityResult> RegisterAsync(CreateRegistrationUserDTO dto, CancellationToken cancellationToken);
        Task<IdentityResult> RegisterAssociatedUserAsync(CreateAssociatedUserDTO dto, CancellationToken cancellationToken);
    }
}
