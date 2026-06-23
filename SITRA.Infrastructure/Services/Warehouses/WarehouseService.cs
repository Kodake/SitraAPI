using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Warehouses;
using SITRA.Infrastructure.Persistence;
using System.Linq;

namespace SITRA.Infrastructure.Services.Warehouses
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public WarehouseService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<Warehouse>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Warehouses.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => x.Name.ToLower().Contains(searchTerm));
            }

            queryable = queryable.OrderBy(x => x.Name);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
