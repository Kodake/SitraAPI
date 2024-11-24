using BackEnd.DTOs;
using BackEnd.Entities.Finances;

namespace BackEnd.Repositories.Finances
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetById(long id);
        Task<List<Currency>> GetCurrencies();
        Task<List<Currency>> GetAll(PaginationDTO paginationDTO);
        Task<long> Create(Currency category);
        Task Update(Currency category);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string code);
    }
}
