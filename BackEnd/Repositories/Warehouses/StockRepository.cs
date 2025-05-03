using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Warehouses
{
    public class StockRepository : RepositoryBase<Stock>, IStockRepository
    {
        private new readonly HttpContext httpContext;
        public StockRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Stock>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Stocks
                .AsQueryable()
                .Include(p => p.Item)
                .Include(p => p.Warehouse);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Item.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Stock?> GetByItemIdAndWarehouseId(long itemId, long warehouseId)
        {
            return await context.Stocks
                .Where(s => s.ItemId == itemId && s.WarehouseId == warehouseId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<double> TotalProductsInStockAsync()
        {
            return await context.Stocks
                .CountAsync(x => x.Qty > 0);
        }
    }
}
