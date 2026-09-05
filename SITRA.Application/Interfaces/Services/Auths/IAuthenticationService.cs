using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Interfaces.Services.Auths
{
    public interface IAuthenticationService
    {
        Task<TokenResponseDTO?> LoginAsync(CreateLoginUserDTO dto, CancellationToken cancellationToken);
        Task<TokenResponseDTO?> RefreshTokenAsync(string expiredToken, string refreshToken, CancellationToken cancellationToken);
        /// <summary>Generates a new JWT + refresh token pair for the given user (by userId). Used after profile updates to refresh claims.</summary>
        Task<TokenResponseDTO?> GenerateTokenForUserAsync(string userId, CancellationToken cancellationToken);
        Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken);
    }
}
