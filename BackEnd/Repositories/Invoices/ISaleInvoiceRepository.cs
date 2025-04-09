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
        Task DeleteInvoiceItemsByInvoiceId(long invoiceId);
        Task<int> TotalSaleInvoicesEmitted();
        Task<double> Last30DaysSales();
    }
}
