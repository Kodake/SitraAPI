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

        public async Task<Currency?> GetById(long id)
        {
            return await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Currency>> GetCurrencies()
        {
            return await context.Currencies
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<long> Create(Currency currency)
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

        public async Task Activate(long id)
        {
            var currency = await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
            if (currency != null)
            {
                currency.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(long id)
        {
            var currency = await context.Currencies.FirstOrDefaultAsync(x => x.Id == id);
            if (currency != null)
            {
                currency.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(long id)
        {
            return await context.Currencies.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(long id, string code)
        {
            return await context.Currencies.AnyAsync(x => x.Id != id && x.Code == code);
        }
    }
}
