using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

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



        public async new Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Items
                .Include(p => p.Brand)
                .Include(p => p.Model)
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.PricingItems)
                    .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                    .ThenInclude(pt => pt.Tax)
                .FirstOrDefaultAsync(p => p.Id == id && p.ItemType == ItemType.Product, cancellationToken);
        }

        public async new Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Product, cancellationToken);
            if (product != null)
            {
                product.IsActive = true;
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async new Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Product, cancellationToken);
            if (product != null)
            {
                product.IsActive = false;
            }
        }

        public async new Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Items.AnyAsync(x => x.Id == id && x.ItemType == ItemType.Product, cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Items.AnyAsync(x => x.Id != id && x.Name == name && x.ItemType == ItemType.Product, cancellationToken);
        }
    }
}




