using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Products
{
    public class PricingItemRepository : RepositoryBase<PricingItem>, IPricingItemRepository
    {
        private new readonly HttpContext httpContext;
        public PricingItemRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<PricingItem>> GetAllAsync(PaginationDTO paginationDTO, Guid itemId, CancellationToken cancellationToken = default)
        {
            var queryable = context.PricingItems
                .Where(x => x.ItemId == itemId)
                .OrderBy(x => x.IsPrincipal);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Pricing)
                .ToListAsync(cancellationToken);

            return items;
        }

        public async Task<PricingItem?> GetPricingItemByItemIdAndPricingId(Guid itemId, Guid pricingId, CancellationToken cancellationToken = default)
        {
            return await context.PricingItems.FirstOrDefaultAsync(pi => pi.ItemId == itemId && pi.PricingId == pricingId, cancellationToken);
        }

        public async new Task<bool> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pricingItemToSetPrincipal = await context.PricingItems
                .Include(pi => pi.Item)
                .FirstOrDefaultAsync(pi => pi.Id == id, cancellationToken);

            if (pricingItemToSetPrincipal == null)
            {
                return false;
            }

            var pricingItems = await context.PricingItems
                .Where(pi => pi.ItemId == pricingItemToSetPrincipal.ItemId && pi.Id != id)
                .ToListAsync(cancellationToken);

            foreach (var pricingItem in pricingItems)
            {
                pricingItem.IsPrincipal = false;
            }

            pricingItemToSetPrincipal.IsPrincipal = true;

            return true;
        }

        public async Task<bool> ExistsAsync(Guid pricingId, Guid itemId, CancellationToken cancellationToken = default)
        {
            return await context.PricingItems.AnyAsync(pi => pi.PricingId == pricingId && pi.ItemId == itemId, cancellationToken);
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var pricingItem = await context.PricingItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (pricingItem != null)
            {
                context.PricingItems.Remove(pricingItem);
            }
        }
    }
}




