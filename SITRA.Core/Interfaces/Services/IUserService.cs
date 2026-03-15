using SITRA.Core.DTOs;
using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync();
        Task<User?> GetUserWithRoleAsync(CancellationToken cancellationToken = default);
        Task<List<User>> GetAllAssociatedUsersAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken);
        Task<int> GetAssociatedUsersCountAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
