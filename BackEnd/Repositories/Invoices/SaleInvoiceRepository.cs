using BackEnd.Context;
using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Invoices
{
    public class SaleInvoiceRepository : RepositoryBase<SaleInvoice>, ISaleInvoiceRepository
    {
        private new readonly HttpContext httpContext;
        public SaleInvoiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<SaleInvoice?> GetByIdAsync(long id)
        {
            return await context.SaleInvoices
                .Include(i => i.Buyer)
                .Include(i => i.Biller)
                .Include(i => i.Pricing)
                .Include(i => i.Warehouse)
                .Include(i => i.Quotation)
                .Include(i => i.SaleInvoiceItems)
                    .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.ItemTaxes)
                    .ThenInclude(x => x.Tax)
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

        public async Task DeleteInvoiceItemsByInvoiceId(long invoiceId)
        {
            var items = await context.SaleInvoiceItems
                .Where(i => i.SaleInvoiceId == invoiceId)
                .ToListAsync();

            if (items.Count != 0)
            {
                context.SaleInvoiceItems.RemoveRange(items);
            }
        }
    }
}
