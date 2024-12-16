using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Accounts
{
    public interface IReceivableRepository : IRepositoryBase<Receivable>
    {
        new Task<List<Receivable>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Receivable?> GetAllByIdAsync(long id);
        Task<Receivable?> GetPaysByIdAsync(long id);
    }
}
