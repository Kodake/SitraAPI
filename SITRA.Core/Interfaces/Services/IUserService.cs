using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync();
        Task<TokenResponseDTO?> LoginAsync(CreateLoginUserDTO dto, CancellationToken cancellationToken);
        Task<IdentityResult> RegisterAsync(CreateRegistrationUserDTO dto, CancellationToken cancellationToken);
        Task<IdentityResult> RegisterAssociatedUserAsync(CreateAssociatedUserDTO dto, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateUserRoleAsync(string userId, UpdateUserRoleDTO dto, CancellationToken cancellationToken);
        Task<bool> ChangeLanguageAsync(Language language, CancellationToken cancellationToken);
        Task<List<User>> GetAllAssociatedUsersAsync(CancellationToken cancellationToken);
    }
}
