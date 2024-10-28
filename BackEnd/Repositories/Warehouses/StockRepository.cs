using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Warehouses
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public StockRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Stock>> GetAll(PaginationDTO paginationDTO)
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

        public async Task<Stock?> GetById(int id)
        {
            return await context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Create(Stock stock)
        {
            context.Add(stock);
            int changes = await context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task Update(Stock stock)
        {
            var existingStock = await context.Stocks.FindAsync(stock.Id);
            if (existingStock != null)
            {
                context.Entry(existingStock).State = EntityState.Detached;
            }

            context.Stocks.Update(stock);
            await context.SaveChangesAsync();
        }

        public async Task<Stock?> GetByItemIdAndWarehouseId(int itemId, int warehouseId)
        {
            return await context.Stocks
                .Where(s => s.ItemId == itemId && s.WarehouseId == warehouseId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
