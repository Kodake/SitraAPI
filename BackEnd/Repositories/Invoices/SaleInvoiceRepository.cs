using BackEnd.Entities.Invoices;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using BackEnd.Context;
using BackEnd.Entities.Contacts;

namespace BackEnd.Repositories.Invoices
{
    public class SaleInvoiceRepository : ISaleInvoiceRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public SaleInvoiceRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }

        public async Task<long> Create(SaleInvoice saleInvoice)
        {
            context.Add(saleInvoice);
            await context.SaveChangesAsync();
            return saleInvoice.Id;
        }

        public async Task<SaleInvoice?> GetById(long id)
        {
            return await context.SaleInvoices
                .Include(i => i.SaleInvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddInvoiceItem(SaleInvoiceItem item)
        {
            await context.SaleInvoiceItems.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task<List<SaleInvoiceItem>> GetItemsByInvoiceId(long invoiceId)
        {
            return await context.SaleInvoiceItems
                .Where(i => i.SaleInvoiceId == invoiceId)
                .ToListAsync();
        }
    }
}
