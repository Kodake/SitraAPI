using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface IPurchaseInvoiceRepository : IRepositoryBase<Invoice>
    {
        new Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Invoice>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddInvoiceItem(InvoiceItem item, CancellationToken cancellationToken = default);
        Task<List<InvoiceItem>> GetItemsByInvoiceId(Guid invoiceId, CancellationToken cancellationToken = default);
        Task DeleteInvoiceItemsByInvoiceId(Guid invoiceId, CancellationToken cancellationToken = default);
        Task<double> Last30DaysPurchasesAsync(CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetPurchasesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetPurchasesReportByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<PurchaseBalanceSummaryDTO> GetBalanceAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    }
}
