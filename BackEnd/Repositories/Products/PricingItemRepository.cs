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

        public async Task<List<PricingItem>> GetAll(PaginationDTO paginationDTO, long itemId)
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

        public async Task<PricingItem?> GetById(long id)
        {
            return await context.PricingItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long> Create(PricingItem pricingItem)
        {
            context.Add(pricingItem);
            await context.SaveChangesAsync();
            return pricingItem.Id;
        }

        public async Task<PricingItem?> GetPricingItemByItemIdAndPricingId(long itemId, long pricingId)
        {
            return await context.PricingItems.FirstOrDefaultAsync(pi => pi.ItemId == itemId && pi.PricingId == pricingId);
        }

        public async Task<PricingItem> Update(PricingItem pricingItem)
        {
            context.PricingItems.Update(pricingItem);
            await context.SaveChangesAsync();
            return pricingItem;
        }

        public async Task<bool> Activate(long id)
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

        public async Task<bool> Exists(long pricingId, long itemId)
        {
            return await context.PricingItems.AnyAsync(pi => pi.PricingId == pricingId && pi.ItemId == itemId);
        }

        public async Task Delete(long id)
        {
            var pricingItem = await context.PricingItems.FirstOrDefaultAsync(x => x.Id == id);
            if (pricingItem != null)
            {
                context.PricingItems.Remove(pricingItem);
                await context.SaveChangesAsync();
            }
        }
    }
}
