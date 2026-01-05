using BackEnd.Context;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        { }

        public async Task<bool> Exists(long id, ItemType itemType)
        {
            return await context.Items
                .AnyAsync(i => i.Id == id && i.ItemType == itemType);
        }

        public async Task<List<Item>> GetByPricingIdAndWarehouseId(long pricingId, long warehouseId)
        {
            return await context.Items
                .Include(x => x.Stocks)
                .Include(x => x.Unit)
                .Include(x => x.ItemTaxes)
                    .ThenInclude(it => it.Tax)
                .Include(x => x.PricingItems)
                    .ThenInclude(pi => pi.Pricing)
                .Include(x => x.ComboItems)
                    .ThenInclude(ci => ci.ChildItem)
                        .ThenInclude(child => child.Stocks)
                .Include(x => x.ComboItems)
                    .ThenInclude(ci => ci.ChildItem)
                        .ThenInclude(child => child.PricingItems)
                .Where(x =>
                    x.PricingItems.Any(pi => pi.PricingId == pricingId) &&
                    (
                        x.ItemType == ItemType.Service ||
                        x.Stocks.Any(s => s.WarehouseId == warehouseId)
                    )
                    || (
                        x.ItemType == ItemType.Combo &&
                        x.ComboItems.Any(ci =>
                            ci.ChildItem.PricingItems.Any(pi => pi.PricingId == pricingId) &&
                            ci.ChildItem.Stocks.Any(s => s.WarehouseId == warehouseId)
                        )
                    )
                )
                .ToListAsync();
        }
    }
}
