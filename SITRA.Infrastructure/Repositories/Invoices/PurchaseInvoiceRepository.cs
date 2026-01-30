using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Invoices;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Invoices
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
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.InvoiceType == InvoiceType.Purchase)
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async new Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await context.Invoices
                .Include(i => i.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Include(i => i.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
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

        public async Task<List<InvoiceItem>> GetItemsByInvoiceId(Guid invoiceId)
        {
            return await context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task DeleteInvoiceItemsByInvoiceId(Guid invoiceId)
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
                .Where(i => i.InvoiceType == InvoiceType.Purchase && i.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(i => i.TotalPrice);
        }

        public async Task<List<Invoice>> GetPurchasesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var queryable = context.Invoices
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
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




