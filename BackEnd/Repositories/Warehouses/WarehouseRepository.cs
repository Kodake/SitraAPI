using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public WarehouseRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Warehouse>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Warehouses.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

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

        public async Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(int productId)
        {
            return await context.Warehouses
                .Where(p => p.Stocks.Any(pi => pi.ItemId == productId))
                .Include(p => p.Stocks)
                .Select(p => new Warehouse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Stocks = p.Stocks,
                }).ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(int id)
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

        public async Task<Warehouse?> GetById(int id)
        {
            return await context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Warehouse warehouse)
        {
            context.Add(warehouse);
            await context.SaveChangesAsync();
            return warehouse.Id;
        }

        public async Task Update(Warehouse warehouse)
        {
            context.Update(warehouse);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var warehouse = await context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (warehouse != null)
            {
                warehouse.IsActive = true;
                context.Entry(warehouse).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var warehouse = await context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (warehouse != null)
            {
                warehouse.IsActive = false;
                context.Entry(warehouse).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Warehouses.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Warehouses.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
