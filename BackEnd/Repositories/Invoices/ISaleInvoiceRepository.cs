using BackEnd.Entities.Invoices;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
{
    public interface ISaleInvoiceRepository : IRepositoryBase<Invoice>
    {
        new Task<Invoice?> GetByIdAsync(long id);
        Task AddInvoiceItem(InvoiceItem item);
        Task<List<InvoiceItem>> GetItemsByInvoiceId(long invoiceId);
        Task DeleteInvoiceItemsByInvoiceId(long invoiceId);
    }
}
