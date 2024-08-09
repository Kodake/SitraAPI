using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Warehouses;
using BackEnd.Helpers;
using BackEnd.Repositories.Contacts;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public WarehouseRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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
                warehouse.IsDeleted = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var warehouse = await context.Warehouses.FirstOrDefaultAsync(x => x.Id == id);
            if (warehouse != null)
            {
                warehouse.IsDeleted = true;
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
