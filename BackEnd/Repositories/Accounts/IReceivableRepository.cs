using BackEnd.DTOs;
using BackEnd.Entities.Accounts;

namespace BackEnd.Repositories.Accounts
{
    public interface IReceivableRepository
    {
        Task<List<Receivable>> GetAll(PaginationDTO paginationDTO);
        Task<long> Create(Receivable receivable);
        Task<bool> Exists(long id);
    }
}
