using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IReceivableRepository : IRepositoryBase<Receivable>
    {
        new Task<List<Receivable>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Receivable?> GetAllByIdAsync(long id);
        Task<Receivable?> GetPaysByIdAsync(long id);
    }
}
