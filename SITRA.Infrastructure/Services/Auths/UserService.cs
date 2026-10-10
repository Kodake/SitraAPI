using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Helpers;
using System.Security.Claims;

namespace SITRA.Infrastructure.Services.Auths
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

        public async Task<List<User>> GetAllAssociatedUsersAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken)
        {
            var queryable = _userManager.Users
                .Include(u => u.TenantRoleUsers)
                .Where(u => u.ParentUserId != null);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(u => 
                    (u.UserName != null && u.UserName.ToLower().Contains(searchTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderBy(u => u.UserName);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetAssociatedUsersCountAsync(CancellationToken cancellationToken = default)
        {
            return await _userManager.Users
                .Where(u => u.ParentUserId != null)
                .CountAsync(cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
