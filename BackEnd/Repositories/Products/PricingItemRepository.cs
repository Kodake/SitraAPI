using BackEnd.Context;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public class PricingItemRepository : IPricingItemRepository
    {
        private readonly ApplicationDbContext context;

        public PricingItemRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
        }
        public async Task<int> Create(PricingItem pricingItem)
        {
            context.Add(pricingItem);
            await context.SaveChangesAsync();
            return pricingItem.Id;
        }
    }
}
