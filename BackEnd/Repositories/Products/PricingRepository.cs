using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class PricingRepository : IPricingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public PricingRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pricing>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Pricings.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Pricing?> GetById(int id)
        {
            return await context.Pricings.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Pricing> Create(Pricing pricing)
        {
            context.Pricings.Add(pricing);
            await context.SaveChangesAsync();
            return pricing;
        }

        public async Task Update(Pricing pricing)
        {
            context.Update(pricing);
            await context.SaveChangesAsync();
        }

        public async Task<Pricing?> GetPricingByName(string? name)
        {
            return await context.Pricings.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Pricings.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Pricings.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
