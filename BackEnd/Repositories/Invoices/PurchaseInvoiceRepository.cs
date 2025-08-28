using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Invoices
{
    public class PurchaseInvoiceRepository : RepositoryBase<Invoice>, IPurchaseInvoiceRepository
    {
        private new readonly HttpContext httpContext;
        public PurchaseInvoiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
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
                .Where(x => x.InvoiceType == Enums.InvoiceType.Purchase)
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

        public async Task<double> Last30DaysPurchasesAsync()
        {
            return await context.Invoices
                .Where(i => i.InvoiceType == Enums.InvoiceType.Purchase && i.CreatedAt >= DateTime.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(i => i.TotalPrice);
        }

        public async Task<List<Invoice>> GetPurchasesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var queryable = context.Invoices
                .Include(x => x.Buyer)
                .Where(x =>
                    x.InvoiceType == InvoiceType.Purchase &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to
                );

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetPurchasesReportByDateRangeAsync(DateTime from, DateTime to)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            return await context.Invoices
                .Where(x =>
                    x.InvoiceType == InvoiceType.Purchase &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to
                ).ToListAsync();
        }
    }
}
