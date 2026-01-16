using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync();
    }
}


