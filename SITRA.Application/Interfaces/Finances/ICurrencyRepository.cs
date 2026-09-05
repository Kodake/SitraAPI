using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Finances
{
    public interface ICurrencyRepository : IRepositoryBase<Currency>
    {
        Task<List<Currency>> GetCurrencies(CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string code, CancellationToken cancellationToken = default);
        Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default);
    }
}
