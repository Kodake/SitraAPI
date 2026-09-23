using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ILogInvoiceRepository : IRepositoryBase<Invoice>
    {
        Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO, string? dgiiStatus = null, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetReadOnlyAsync(PaginationDTO paginationDTO, string? dgiiStatus = null, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetPendingSyncInvoicesAsync(Guid? tenantId = null, CancellationToken cancellationToken = default);
        Task<Invoice?> GetInvoiceForSyncAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
