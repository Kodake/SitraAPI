using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.AdministrativeTools
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        private new readonly HttpContext httpContext;
        public RoleRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Role>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Set<Role>()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => !r.IsSystemRole)
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);

            return items;
        }

        public async new Task<List<Role>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Set<Role>()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => !r.IsSystemRole)
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);

            return items;
        }

        public async Task<List<Role>> GetAll(
            bool? hasPermissions = null,
            CancellationToken cancellationToken = default)
        {
            var query = context.Set<Role>()
                .Where(r => !r.IsSystemRole);

            if (hasPermissions.HasValue)
            {
                query = hasPermissions.Value
                    ? query.Where(r => r.RolePermissions.Any())
                    : query.Where(r => !r.RolePermissions.Any());
            }

            query = query
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);

            return await query
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async new Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Set<Role>()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => r.Id == id)
                .Where(r => !r.IsSystemRole)
                .OrderBy(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await GetByIdAsync(id, cancellationToken);
            if (role == null || role.IsSystemRole) return;
            context.Set<Role>().Remove(role);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.TenantRoles.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }
    }
}

