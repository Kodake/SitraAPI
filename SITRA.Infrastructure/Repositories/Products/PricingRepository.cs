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

        public async Task<List<Pricing>> GetAll(CancellationToken cancellationToken = default)
        {
            return await context.Pricings
                .ToListAsync(cancellationToken);
        }

        public async Task<Pricing?> GetPricingByName(string? name, CancellationToken cancellationToken = default)
        {
            return await context.Pricings.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItem(Guid itemId, CancellationToken cancellationToken = default)
        {
            return await context.Pricings
                .Where(p => !p.PricingItems.Any(pi => pi.ItemId == itemId))
                .Select(p => new Pricing
                {
                    Id = p.Id,
                    Name = p.Name,
                    PricingType = p.PricingType,
                    Percentage = p.Percentage
                }).ToListAsync(cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Pricings.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }

        public async Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default)
        {
            var pricing = await context.Pricings.FindAsync(new object[] { id }, cancellationToken);
            if (pricing == null) return false;

            var defaults = await context.Pricings
                .Where(x => x.IsDefault && x.Id != id)
                .ToListAsync(cancellationToken);

            foreach (var p in defaults)
                p.IsDefault = false;

            pricing.IsDefault = true;
            return true;
        }
    }
}





