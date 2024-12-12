using BackEnd.Entities.Finances;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Finances
{
    public interface ICurrencyRepository : IRepositoryBase<Currency>
    {
        Task<List<Currency>> GetCurrencies();
        Task<bool> Exists(long id, string code);
    }
}
