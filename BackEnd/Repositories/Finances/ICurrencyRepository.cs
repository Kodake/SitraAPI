using BackEnd.DTOs;
using BackEnd.Entities.Finances;

namespace BackEnd.Repositories.Finances
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetById(int id);
        Task<List<Currency>> GetAll(PaginationDTO paginationDTO);
        Task<int> Create(Currency category);
        Task Update(Currency category);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
