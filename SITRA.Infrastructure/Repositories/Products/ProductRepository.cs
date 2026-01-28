using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Products
{
    public class ProductRepository : RepositoryBase<Item>, IProductRepository
    {
        private new readonly HttpContext httpContext;
        public ProductRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Items
                .Where(x => x.ItemType == ItemType.Product)
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
                .Where(u => u.ItemType == ItemType.Product);

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

        public async Task<IEnumerable<Item>> GetAll()
        {
            return await context.Items
                .Where(p => p.Stocks.Any(x => x.Item.ItemType == ItemType.Product))
                .Select(p => new Item
                {
                    Id = p.Id,
                    Name = p.Name,
                    Reference = p.Reference,
                    Barcode = p.Barcode,
                }).ToListAsync();
        }

        public async new Task<Item?> GetByIdAsync(Guid id)
        {
            return await context.Items
                .Include(p => p.ItemUnit)
                    .ThenInclude(iu => iu.Unit)
                .Include(p => p.ItemBrand)
                    .ThenInclude(ib => ib.Brand)
                .Include(p => p.ItemModel)
                    .ThenInclude(im => im.Model)
                .Include(p => p.ItemCategory)
                    .ThenInclude(ic => ic.Category)
                .Include(p => p.ItemSubCategory)
                    .ThenInclude(isc => isc.SubCategory)
                .Include(p => p.PricingItems)
                    .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                    .ThenInclude(pt => pt.Tax)
                .FirstOrDefaultAsync(p => p.Id == id && p.ItemType == ItemType.Product);
        }

        public async new Task ActivateAsync(Guid id)
        {
            var product = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Product);
            if (product != null)
            {
                product.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async new Task DeactivateAsync(Guid id)
        {
            var product = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Product);
            if (product != null)
            {
                product.IsActive = false;
            }
        }

        public async new Task<bool> ExistsAsync(Guid id)
        {
            return await context.Items.AnyAsync(x => x.Id == id && x.ItemType == ItemType.Product);
        }

        public async Task<bool> Exists(Guid id, string name)
        {
            return await context.Items.AnyAsync(x => x.Id != id && x.Name == name && x.ItemType == ItemType.Product);
        }
    }
}




