using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Finances
{
    public interface ICurrencyRepository : IRepositoryBase<Currency>
    {
        Task<List<Currency>> GetCurrencies();
        Task<bool> Exists(long id, string code);
    }
}
