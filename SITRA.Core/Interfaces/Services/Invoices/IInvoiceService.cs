using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.Interfaces.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetValidSaleInvoicesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetValidPurchaseInvoicesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<SpotPayable>> GetValidSpotPayablesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<SpotPayment>> GetValidSpotPaymentsAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
