using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Warehouses;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Warehouses
{
    public class WarehouseRepository : RepositoryBase<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Warehouse>> GetAll(StatusType status, CancellationToken cancellationToken = default)
        {
            IQueryable<Warehouse> query = context.Warehouses.AsQueryable();

            switch (status)
            {
                case StatusType.Activated:
                    query = query.Where(u => u.IsActive);
                    break;
                case StatusType.Deactivated:
                    query = query.Where(u => !u.IsActive);
                    break;
                case StatusType.All:
                default:
                    break;
            }

            return await query.OrderBy(u => u.Id).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(Guid productId, CancellationToken cancellationToken = default)
        {
            return await context.Warehouses
                .Where(p => p.Stocks.Any(pi => pi.ItemId == productId))
                .Select(p => new Warehouse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stocks = p.Stocks.Where(stock => stock.ItemId == productId).ToList(),
                }).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Warehouses
                .Where(pi => pi.Id != id)
                .Select(p => new Warehouse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stocks = p.Stocks,
                }).ToListAsync(cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Warehouses.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }

        public async Task<int> TotalWarehousesAddedAsync(CancellationToken cancellationToken = default)
        {
            return await context.Warehouses.CountAsync(x => x.IsActive, cancellationToken);
        }

        public async Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default)
        {
            var warehouse = await context.Warehouses.FindAsync(new object[] { id }, cancellationToken);
            if (warehouse == null) return false;

            var defaults = await context.Warehouses
                .Where(x => x.IsDefault && x.Id != id)
                .ToListAsync(cancellationToken);

            foreach (var w in defaults)
                w.IsDefault = false;

            warehouse.IsDefault = true;
            return true;
        }
    }
}





