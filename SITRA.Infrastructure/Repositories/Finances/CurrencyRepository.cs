using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces.Finances;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Finances
{
    public class CurrencyRepository : RepositoryBase<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Currency>> GetCurrencies(CancellationToken cancellationToken = default)
        {
            return await context.Currencies
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string code, CancellationToken cancellationToken = default)
        {
            return await context.Currencies.AnyAsync(x => x.Id != id && x.Code == code, cancellationToken);
        }

        public async Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default)
        {
            var currency = await context.Currencies.FindAsync(new object[] { id }, cancellationToken);
            if (currency == null) return false;

            var defaults = await context.Currencies
                .Where(x => x.IsDefault && x.Id != id)
                .ToListAsync(cancellationToken);

            foreach (var c in defaults)
                c.IsDefault = false;

            currency.IsDefault = true;
            return true;
        }
    }
}





