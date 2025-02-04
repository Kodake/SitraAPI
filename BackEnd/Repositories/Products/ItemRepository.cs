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

        public async Task<List<Item>> GetByPricingIdAndWarehouseId(long pricingId, long warehouseId)
        {
            return await context.Items
                .Include(x => x.Stocks)
                .Include(x => x.Unit)
                .Include(x => x.ItemTaxes)
                    .ThenInclude(it => it.Tax)
                .Include(x => x.PricingItems)
                    .ThenInclude(pi => pi.Pricing)
                .Where(x =>
                    x.PricingItems.Any(pi => pi.PricingId == pricingId) &&
                    (
                        x.ItemType == ItemType.Service ||
                        x.Stocks.Any(s => s.WarehouseId == warehouseId)
                    )
                )
                .ToListAsync();
        }
    }
}
