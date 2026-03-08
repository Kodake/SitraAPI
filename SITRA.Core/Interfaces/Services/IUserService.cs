using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync();
        Task<User?> GetUserWithRoleAsync(CancellationToken cancellationToken = default);
        Task<List<User>> GetAllAssociatedUsersAsync(CancellationToken cancellationToken);
    }
}
