using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.Interfaces.Warehouses;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Warehouses
{
    public class WarehouseRepository : RepositoryBase<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Warehouse>> GetAll(StatusType status)
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

            return await query.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(Guid productId)
        {
            return await context.Warehouses
                .Where(p => p.Stocks.Any(pi => pi.ItemId == productId))
                .Select(p => new Warehouse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stocks = p.Stocks.Where(stock => stock.ItemId == productId).ToList(),
                }).ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(Guid id)
        {
            return await context.Warehouses
                .Where(pi => pi.Id != id)
                .Select(p => new Warehouse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stocks = p.Stocks,
                }).ToListAsync();
        }

        public async Task<bool> Exists(Guid id, string name)
        {
            return await context.Warehouses.AnyAsync(x => x.Id != id && x.Name == name);
        }

        public async Task<int> TotalWarehousesAddedAsync()
        {
            return await context.Warehouses.CountAsync(x => x.IsActive);
        }
    }
}





