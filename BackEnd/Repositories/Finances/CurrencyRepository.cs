using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Finances;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Finances
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public CurrencyRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Currency>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Currencies.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Currency?> GetById(int id)
        {
            return await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Currency currency)
        {
            context.Add(currency);
            await context.SaveChangesAsync();
            return currency.Id;
        }

        public async Task Update(Currency currency)
        {
            context.Update(currency);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var currency = await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
            if (currency != null)
            {
                currency.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var currency = await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
            if (currency != null)
            {
                currency.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Currencies.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string code)
        {
            return await context.Currencies.AnyAsync(x => x.Id != id && x.Code == code);
        }
    }
}
