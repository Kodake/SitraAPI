using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ISpotPayableRepository : IRepositoryBase<SpotPayable>
    {
        new Task<List<SpotPayable>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<SpotPayable>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<SpotPayable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
