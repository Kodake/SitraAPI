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
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public StockService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<Stock>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            IQueryable<Stock> queryable = _context.Stocks.AsNoTracking()
                .Include(p => p.Item)
                .Include(p => p.Warehouse);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.Item != null && x.Item.Name.ToLower().Contains(searchTerm)) ||
                    (x.Warehouse != null && x.Warehouse.Name.ToLower().Contains(searchTerm))
                );
            }

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .OrderBy(x => x.Item.Name)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
