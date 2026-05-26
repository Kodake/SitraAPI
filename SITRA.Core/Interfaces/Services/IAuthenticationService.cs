using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDTO?> LoginAsync(CreateLoginUserDTO dto, CancellationToken cancellationToken);
        Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
    }
}
