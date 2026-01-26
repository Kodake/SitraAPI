using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ISaleInvoiceRepository : IRepositoryBase<Invoice>
    {
        new Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<Invoice?> GetByIdAsync(long id);
        Task AddInvoiceItem(InvoiceItem item);
        Task<List<InvoiceItem>> GetItemsByInvoiceId(long invoiceId);
        Task DeleteInvoiceItemsByInvoiceId(long? invoiceId);
        Task<int> Last24HoursSaleInvoicesAsync();
        Task<double> Last30DaysSalesAsync();
        Task<List<Invoice>> GetSalesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO);
        Task<List<Invoice>> GetSalesReportByDateRangeAsync(DateTime from, DateTime to);
        Task<BalanceSummaryDTO> GetBalanceAsync(DateTimeOffset from, DateTimeOffset to);
    }
}
