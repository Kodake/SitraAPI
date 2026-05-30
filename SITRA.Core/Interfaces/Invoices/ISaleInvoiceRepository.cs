using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ISaleInvoiceRepository : IRepositoryBase<Invoice>
    {

        new Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddInvoiceItem(InvoiceItem item, CancellationToken cancellationToken = default);
        Task<List<InvoiceItem>> GetItemsByInvoiceId(Guid invoiceId, CancellationToken cancellationToken = default);
        Task DeleteInvoiceItemsByInvoiceId(Guid? invoiceId, CancellationToken cancellationToken = default);

        Task<List<Invoice>> GetSalesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetSalesReportByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}
