using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ServiceRepository : RepositoryBase<Item>, IServiceRepository
    {
        private new readonly HttpContext httpContext;
        public ServiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO)
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

        public async new Task<Item?> GetByIdAsync(long id)
        {
            return await context.Items
                .Where(x => x.ItemType == ItemType.Service)
                .Include(p => p.Unit)
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.PricingItems)
                    .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                    .ThenInclude(pt => pt.Tax)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async new Task ActivateAsync(long id)
        {
            var service = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Service);
            if (service != null)
            {
                service.IsActive = true;
            }
        }

        public async new Task DeactivateAsync(long id)
        {
            var service = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Service);
            if (service != null)
            {
                service.IsActive = false;
            }
        }

        public async new Task<bool> ExistsAsync(long id)
        {
            return await context.Items.AnyAsync(x => x.Id == id && x.ItemType == ItemType.Service);
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Items.AnyAsync(x => x.Id != id && x.Name == name && x.ItemType == ItemType.Service);
        }
    }
}
