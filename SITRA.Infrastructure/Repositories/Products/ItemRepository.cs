using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Products
{
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        { }

        public async Task<bool> Exists(Guid id, ItemType itemType, CancellationToken cancellationToken = default)
        {
            return await context.Items
                .AnyAsync(i => i.Id == id && i.ItemType == itemType, cancellationToken);
        }

        public async new Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Items
                .Include(p => p.PricingItems)
                    .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                    .ThenInclude(pt => pt.Tax)
                .Include(p => p.ComboItems)
                    .ThenInclude(ci => ci.ChildItem)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }


    }
}