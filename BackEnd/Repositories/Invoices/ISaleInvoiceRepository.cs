using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore.Storage;

namespace BackEnd.Repositories.Invoices
{
    public interface ISaleInvoiceRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<SaleInvoice?> GetById(long id);
        Task<long> Create(SaleInvoice saleInvoice);
        Task AddInvoiceItem(SaleInvoiceItem item);
        Task<List<SaleInvoiceItem>> GetItemsByInvoiceId(long invoiceId);
    }
}
