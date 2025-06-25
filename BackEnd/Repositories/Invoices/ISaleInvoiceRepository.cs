using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
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
    }
}
