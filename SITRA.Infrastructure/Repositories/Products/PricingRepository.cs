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

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Pricings.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }
    }
}





