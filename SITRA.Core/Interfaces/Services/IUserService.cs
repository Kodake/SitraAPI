using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync();
        Task<List<User>> GetAllAssociatedUsersAsync(CancellationToken cancellationToken);
    }
}
