using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IPayableRepository : IRepositoryBase<Payable>
    {
        new Task<List<Payable>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Payable?> GetAllByIdAsync(Guid id);
        Task<Payable?> GetPaysByIdAsync(Guid id);
    }
}
