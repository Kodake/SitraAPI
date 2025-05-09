using BackEnd.Context;
using BackEnd.Entities.Auths;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Services
{
    public class UserService : RepositoryBase<User>, IUserService
    {
        private new readonly HttpContext httpContext;
        private readonly UserManager<User> userManager;

        public UserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager)
            : base(context, httpContextAccessor)
        {
            this.httpContext = httpContextAccessor.HttpContext!;
            this.userManager = userManager;
        }

        public async Task<User?> GetUserAsync()
        {
            var emailClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == "email");

            if (emailClaim is null)
                return null;

            var email = emailClaim.Value;
            return (User?)await userManager.FindByEmailAsync(email);
        }
    }
}
