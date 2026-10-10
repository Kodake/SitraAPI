using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.AdministrativeTools
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public RoleService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Role>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Set<Role>()
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => !r.IsSystemRole);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(r => 
                    (r.Name != null && r.Name.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderBy(x => x.Name);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            var items = await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
