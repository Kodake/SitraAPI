using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IPayableRepository : IRepositoryBase<Payable>
    {
        new Task<List<Payable>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Payable>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<Payable?> GetAllByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

