using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public ServiceRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Item>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Items
                .Where(x => x.ItemType == ItemType.Service)
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.PricingItems)
                .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                .ThenInclude(pt => pt.Tax)
                .ToListAsync();

            return items;
        }

        public async Task<List<Item>> GetAll(StatusType status)
        {
            IQueryable<Item> query = context.Items
                .Where(u => u.ItemType == ItemType.Service);

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

        public async Task<Item?> GetById(long id)
        {
            return await context.Items
                .Where(x => x.ItemType == ItemType.Service)
                .Include(p => p.Unit)
                .Include(p => p.Category)
                .Include(p => p.PricingItems)
                    .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                    .ThenInclude(pt => pt.Tax)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<long> Create(Item service)
        {
            context.Add(service);
            await context.SaveChangesAsync();
            return service.Id;
        }

        public async Task Update(Item service)
        {
            context.Update(service);
            await context.SaveChangesAsync();
        }

        public async Task Activate(long id)
        {
            var service = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Service);
            if (service != null)
            {
                service.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(long id)
        {
            var service = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Service);
            if (service != null)
            {
                service.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(long id)
        {
            return await context.Items.AnyAsync(x => x.Id == id && x.ItemType == ItemType.Service);
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Items.AnyAsync(x => x.Id != id && x.Name == name && x.ItemType == ItemType.Service);
        }
    }
}
