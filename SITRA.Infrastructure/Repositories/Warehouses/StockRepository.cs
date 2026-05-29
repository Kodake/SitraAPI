using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Warehouses;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Warehouses
{
    public class StockRepository : RepositoryBase<Stock>, IStockRepository
    {
        private new readonly HttpContext httpContext;
        public StockRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Stock>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Stocks
                .AsQueryable()
                .Include(p => p.Item)
                .Include(p => p.Warehouse);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Item.Name)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<List<Stock>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Stocks.AsNoTracking()
                .Include(p => p.Item)
                .Include(p => p.Warehouse);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Item.Name)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Stock>> GetAllByWarehouseIdsAsync(List<Guid> warehouseIds, PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Stocks
                .AsQueryable()
                .Where(s => warehouseIds.Contains(s.WarehouseId))
                .Include(p => p.Item)
                .Include(p => p.Warehouse);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Item.Name)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Stock>> GetStockByWarehouseIdsAsync(List<Guid> warehouseIds, CancellationToken cancellationToken = default)
        {
            return await context.Stocks
                .Where(s => warehouseIds.Contains(s.WarehouseId))
                .Include(p => p.Item)
                .Include(p => p.Warehouse)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Stock?> GetByItemIdAndWarehouseId(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default)
        {
            return await context.Stocks
                .Where(s => s.ItemId == itemId && s.WarehouseId == warehouseId)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<double> TotalProductsInStockAsync(CancellationToken cancellationToken = default)
        {
            return await context.Stocks
                .CountAsync(x => x.Qty > 0, cancellationToken);
        }
    }
}




