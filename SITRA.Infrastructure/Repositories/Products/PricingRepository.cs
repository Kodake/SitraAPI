using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Products
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

        public async Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(Guid itemId)
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

        public async Task<bool> Exists(Guid id, string name)
        {
            return await context.Pricings.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}





