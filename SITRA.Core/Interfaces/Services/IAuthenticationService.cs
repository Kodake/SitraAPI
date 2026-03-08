using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDTO?> LoginAsync(CreateLoginUserDTO dto, CancellationToken cancellationToken);
    }
}
