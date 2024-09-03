using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class PricingItemRepository : IPricingItemRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public PricingItemRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<PricingItem>> GetAll(PaginationDTO paginationDTO, int itemId)
        {
            var queryable = context.PricingItems
                .AsNoTracking()
                .Where(x => x.ItemId == itemId)
                .OrderBy(x => x.IsPrincipal);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Pricing)
                .ToListAsync();

            return items;
        }

        public async Task<int> Create(PricingItem pricingItem)
        {
            context.Add(pricingItem);
            await context.SaveChangesAsync();
            return pricingItem.Id;
        }

        public async Task<PricingItem?> GetPricingItemByItemIdAndPricingId(int itemId, int pricingId)
        {
            return await context.PricingItems.FirstOrDefaultAsync(pi => pi.ItemId == itemId && pi.PricingId == pricingId);
        }

        public async Task<PricingItem> Update(PricingItem pricingItem)
        {
            context.PricingItems.Update(pricingItem);
            await context.SaveChangesAsync();
            return pricingItem;
        }

        public async Task<bool> Activate(int id)
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

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Exists(int pricingId, int itemId)
        {
            return await context.PricingItems
                .AnyAsync(pi => pi.PricingId == pricingId && pi.ItemId == itemId);
        }
    }
}
