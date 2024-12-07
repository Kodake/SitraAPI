using BackEnd.DTOs;
using BackEnd.Entities.Accounts;

namespace BackEnd.Repositories.Accounts
{
    public interface IReceivableRepository
    {
        Task<List<Receivable>> GetAll(PaginationDTO paginationDTO);
        Task<Receivable?> GetAllById(long id);
        Task<long> Create(Receivable receivable);
        Task<bool> Exists(long id);
        Task<Receivable?> GetById(long id);
        Task Update(Receivable receivable);
        Task<Receivable?> GetPaysById(long id);
    }
}
