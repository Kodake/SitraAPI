using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Accounts
{
    public interface IPayableRepository : IRepositoryBase<Payable>
    {
        new Task<List<Payable>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Payable?> GetAllByIdAsync(long id);
        Task<Payable?> GetPaysByIdAsync(long id);
    }
}
