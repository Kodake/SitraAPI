using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services
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
            var emailClaim = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email");

            if (emailClaim is null)
                return null;

            var email = emailClaim.Value;
            return (User?)await userManager.FindByEmailAsync(email);
        }
    }
}

