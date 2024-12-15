using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class PricingItemRepository : RepositoryBase<PricingItem>, IPricingItemRepository
    {
        private new readonly HttpContext httpContext;
        public PricingItemRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<PricingItem>> GetAllAsync(PaginationDTO paginationDTO, long itemId)
        {
            var queryable = context.PricingItems
                .Where(x => x.ItemId == itemId)
                .OrderBy(x => x.IsPrincipal);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Pricing)
                .ToListAsync();

            return items;
        }

        public async Task<PricingItem?> GetPricingItemByItemIdAndPricingId(long itemId, long pricingId)
        {
            return await context.PricingItems.FirstOrDefaultAsync(pi => pi.ItemId == itemId && pi.PricingId == pricingId);
        }

        public async new Task<bool> ActivateAsync(long id)
        {
            var pricingItemToSetPrincipal = await context.PricingItems
                .Include(pi => pi.Item)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (pricingItemToSetPrincipal == null)
            {
                return false;
            }

            var pricingItems = await context.PricingItems
                .Where(pi => pi.ItemId == pricingItemToSetPrincipal.ItemId && pi.Id != id)
                .ToListAsync();

            foreach (var pricingItem in pricingItems)
            {
                pricingItem.IsPrincipal = false;
            }

            pricingItemToSetPrincipal.IsPrincipal = true;

            return true;
        }

        public async Task<bool> ExistsAsync(long pricingId, long itemId)
        {
            return await context.PricingItems.AnyAsync(pi => pi.PricingId == pricingId && pi.ItemId == itemId);
        }

        public async Task Delete(long id)
        {
            var pricingItem = await context.PricingItems.FirstOrDefaultAsync(x => x.Id == id);
            if (pricingItem != null)
            {
                context.PricingItems.Remove(pricingItem);
            }
        }
    }
}
