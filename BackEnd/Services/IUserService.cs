using BackEnd.Entities.Auths;

namespace BackEnd.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync();
    }
}
