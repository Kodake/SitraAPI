using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
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



        public async new Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Invoices
                .Include(i => i.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Include(i => i.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
                .Include(i => i.InvoiceFiscalSupport)
                    .ThenInclude(ifs => ifs.FiscalSupport)
                .Include(i => i.Pricing)
                .Include(i => i.Warehouse)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.ItemTaxes)
                    .ThenInclude(x => x.Tax)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task AddInvoiceItem(InvoiceItem item, CancellationToken cancellationToken = default)
        {
            await context.InvoiceItems.AddAsync(item, cancellationToken);
        }

        public async Task<List<InvoiceItem>> GetItemsByInvoiceId(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteInvoiceItemsByInvoiceId(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var items = await context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync(cancellationToken);

            if (items.Count != 0)
            {
                context.InvoiceItems.RemoveRange(items);
            }
        }



        public async Task<List<Invoice>> GetPurchasesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var queryable = context.Invoices
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Where(x =>
                    x.InvoiceType == InvoiceType.Purchase &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to &&
                    (x.SpotPayable != null || x.Payable != null)
                );

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetPurchasesReportByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            return await context.Invoices
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Where(x =>
                    x.InvoiceType == InvoiceType.Purchase &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to &&
                    (x.SpotPayable != null || x.Payable != null)
                ).ToListAsync(cancellationToken);
        }

    }
}




