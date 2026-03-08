using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<User> _userManager;

        public UserService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager)
            : base(context, httpContextAccessor)
        {
            this.httpContext = httpContextAccessor.HttpContext!;
            _userManager = userManager;
        }

        public async Task<User?> GetUserAsync()
        {
            var emailClaim = httpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email");

            if (emailClaim is null)
                return null;

            return (User?)await _userManager.FindByEmailAsync(emailClaim.Value);
        }

        public async Task<User?> GetUserWithRoleAsync(CancellationToken cancellationToken = default)
        {
            var emailClaim = httpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email");

            if (emailClaim is null)
                return null;

            return await context.Set<User>()
                .Include(u => u.TenantRoleUsers)
                .FirstOrDefaultAsync(u => u.Email == emailClaim.Value, cancellationToken);
        }

        public async Task<List<User>> GetAllAssociatedUsersAsync(CancellationToken cancellationToken)
        {
            var queryable = _userManager.Users
                .Include(u => u.TenantRoleUsers)
                .Where(u => u.ParentUserId != null);

            return await queryable
                .OrderBy(u => u.UserName)
                .ToListAsync(cancellationToken);
        }
    }
}
