using BackEnd.Context;
using BackEnd.Entities.Finances;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Finances
{
    public class CurrencyRepository : RepositoryBase<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Currency>> GetCurrencies()
        {
            return await context.Currencies
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<bool> Exists(long id, string code)
        {
            return await context.Currencies.AnyAsync(x => x.Id != id && x.Code == code);
        }
    }
}
