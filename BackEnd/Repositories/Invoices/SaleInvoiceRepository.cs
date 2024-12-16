using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using BackEnd.Context;

namespace BackEnd.Repositories.Invoices
{
    public class SaleInvoiceRepository : RepositoryBase<SaleInvoice>, ISaleInvoiceRepository
    {
        public SaleInvoiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async new Task<SaleInvoice?> GetByIdAsync(long id)
        {
            return await context.SaleInvoices
                .Include(i => i.SaleInvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddInvoiceItem(SaleInvoiceItem item)
        {
            await context.SaleInvoiceItems.AddAsync(item);
        }

        public async Task<List<SaleInvoiceItem>> GetItemsByInvoiceId(long invoiceId)
        {
            return await context.SaleInvoiceItems
                .Where(i => i.SaleInvoiceId == invoiceId)
                .ToListAsync();
        }
    }
}
