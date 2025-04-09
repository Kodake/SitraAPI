using BackEnd.Context;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Warehouses
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

        public async Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(long productId)
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

        public async Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(long id)
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

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Warehouses.AnyAsync(x => x.Id != id && x.Name == name);
        }

        public async Task<int> TotalWarehousesAdded()
        {
            return await context.Warehouses.CountAsync(x => x.IsActive);
        }
    }
}
