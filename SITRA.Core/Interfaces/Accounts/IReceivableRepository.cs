using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IReceivableRepository : IRepositoryBase<Receivable>
    {
        new Task<List<Receivable>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Receivable>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<Receivable?> GetAllByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Receivable?> GetPaysByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
