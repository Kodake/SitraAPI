using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserRoleService
    {
        Task<IdentityResult> UpdateUserRoleAsync(string userId, UpdateUserRoleDTO dto, CancellationToken cancellationToken);
        Task<bool> ChangeLanguageAsync(Language language, CancellationToken cancellationToken);
        Task<bool> UpdateProfileNameAsync(string userName, CancellationToken cancellationToken);
    }
}
