using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Interfaces.Services.Auths
{
    public interface IUserRegistrationService
    {
        Task<RegistrationDTO> RegisterAsync(CreateRegistrationUserDTO dto, CancellationToken cancellationToken);
        Task<RegistrationDTO> RegisterAssociatedUserAsync(CreateAssociatedUserDTO dto, CancellationToken cancellationToken);
    }
}
