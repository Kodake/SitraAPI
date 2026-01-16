using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.Interfaces.Finances;
using SITRA.Core.Entities.Finances;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Finances
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





