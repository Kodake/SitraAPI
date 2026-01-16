using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface IPurchaseInvoiceRepository : IRepositoryBase<Invoice>
    {
        new Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<Invoice?> GetByIdAsync(long id);
        Task AddInvoiceItem(InvoiceItem item);
        Task<List<InvoiceItem>> GetItemsByInvoiceId(long invoiceId);
        Task DeleteInvoiceItemsByInvoiceId(long invoiceId);
        Task<double> Last30DaysPurchasesAsync();
        Task<List<Invoice>> GetPurchasesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO);
        Task<List<Invoice>> GetPurchasesReportByDateRangeAsync(DateTime from, DateTime to);
    }
}
