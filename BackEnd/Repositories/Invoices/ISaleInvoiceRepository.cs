using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Invoices
{
    public interface ISaleInvoiceRepository : IRepositoryBase<SaleInvoice>
    {
        new Task<SaleInvoice?> GetByIdAsync(long id);
        Task AddInvoiceItem(SaleInvoiceItem item);
        Task<List<SaleInvoiceItem>> GetItemsByInvoiceId(long invoiceId);
    }
}
