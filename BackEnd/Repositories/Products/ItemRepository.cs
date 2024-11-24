using BackEnd.Context;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ItemRepository: IItemRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public ItemRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Item>> GetByPricingIdAndWarehouseId(int pricingId, int warehouseId)
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
                        x.Stocks.Any(s => s.WarehouseId == warehouseId && s.Qty > 0)
                    )
                )
                .ToListAsync();
        }
    }
}
