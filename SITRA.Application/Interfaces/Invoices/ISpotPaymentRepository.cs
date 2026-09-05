using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ISpotPaymentRepository : IRepositoryBase<SpotPayment>
    {
        new Task<List<SpotPayment>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<SpotPayment>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<SpotPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
