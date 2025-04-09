using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Invoices
{
    public class SaleInvoiceRepository : RepositoryBase<Invoice>, ISaleInvoiceRepository
    {
        private new readonly HttpContext httpContext;
        public SaleInvoiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Invoices
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.InvoiceType == Enums.InvoiceType.Sale)
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async new Task<Invoice?> GetByIdAsync(long id)
        {
            return await context.Invoices
                .Include(i => i.Buyer)
                .Include(i => i.Biller)
                .Include(i => i.Pricing)
                .Include(i => i.Warehouse)
                .Include(i => i.Quotation)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.ItemTaxes)
                    .ThenInclude(x => x.Tax)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddInvoiceItem(InvoiceItem item)
        {
            await context.InvoiceItems.AddAsync(item);
        }

        public async Task<List<InvoiceItem>> GetItemsByInvoiceId(long invoiceId)
        {
            return await context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task DeleteInvoiceItemsByInvoiceId(long invoiceId)
        {
            var items = await context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync();

            if (items.Count != 0)
            {
                context.InvoiceItems.RemoveRange(items);
            }
        }

        public async Task<int> TotalSaleInvoicesEmitted()
        {
            return await context.Invoices
                .Where(x => x.InvoiceType == Enums.InvoiceType.Sale)
                .CountAsync();
        }

        public async Task<double> Last30DaysSales()
        {
            return await context.Invoices
                .Where(x => x.InvoiceType == Enums.InvoiceType.Sale)
                // && x.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .SumAsync(x => x.TotalPrice);
        }
    }
}
