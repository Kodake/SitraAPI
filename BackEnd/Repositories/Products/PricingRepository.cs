using BackEnd.Context;
using BackEnd.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class PricingRepository : RepositoryBase<Pricing>, IPricingRepository
    {
        public PricingRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Pricing>> GetAll()
        {
            return await context.Pricings
                .ToListAsync();
        }

        public async Task<Pricing?> GetPricingByName(string? name)
        {
            return await context.Pricings.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(long itemId)
        {
            return await context.Pricings
                .Where(p => !p.PricingItems.Any(pi => pi.ItemId == itemId))
                .Select(p => new Pricing
                {
                    Id = p.Id,
                    Name = p.Name,
                    PricingType = p.PricingType,
                    Percentage = p.Percentage
                }).ToListAsync();
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Pricings.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
