using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Invoices;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Invoices.SaleInvoices;

namespace SITRA.Infrastructure.Repositories.Invoices
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
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Include(x => x.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.InvoiceType == InvoiceType.Sale)
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
                .Include(i => i.Quotation)

                // SpotPayment + Payments
                .Include(i => i.SpotPayment!)
                    .ThenInclude(sp => sp.Payments)

                // Invoice Items
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Item)
                        .ThenInclude(it => it.ItemTaxes)
                            .ThenInclude(itx => itx.Tax)

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

        public async Task DeleteInvoiceItemsByInvoiceId(Guid? invoiceId)
        {
            var items = await context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync();

            if (items.Count != 0)
            {
                context.InvoiceItems.RemoveRange(items);
            }
        }

        public async Task<int> Last24HoursSaleInvoicesAsync()
        {
            return await context.Invoices
                .Where(x => x.InvoiceType == InvoiceType.Sale && x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last24Hours))
                .CountAsync();
        }

        public async Task<double> Last30DaysSalesAsync()
        {
            return await context.Invoices
                .Where(x => x.InvoiceType == InvoiceType.Sale && x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(x => x.TotalPrice);
        }

        public async Task<List<Invoice>> GetSalesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var queryable = context.Invoices
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Include(x => x.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
                .Where(x =>
                    x.InvoiceType == InvoiceType.Sale &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to
                );

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetSalesReportByDateRangeAsync(DateTime from, DateTime to)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            return await context.Invoices
                .Where(x =>
                    x.InvoiceType == InvoiceType.Sale &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to
                ).ToListAsync();
        }

        public async Task<BalanceSummaryDTO> GetBalanceAsync(DateTimeOffset from, DateTimeOffset to)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var expenses = await context.Expenses
                .Where(x => x.Date >= from && x.Date <= to && x.IsActive)
                .SumAsync(x => x.Amount);

            var spotSales = await context.SpotPayments
                .Include(sp => sp.Invoice)
                .Where(sp => sp.Invoice.CreatedAt >= from && sp.Invoice.CreatedAt <= to && sp.IsActive)
                .SumAsync(sp => sp.Invoice.TotalPrice);

            var receivablePayments = await context.PaymentReceipts
                .Where(pr => pr.CreatedAt >= from && pr.CreatedAt <= to && pr.IsActive)
                .SumAsync(pr => pr.Amount);

            var totalIncome = spotSales + receivablePayments;

            return new BalanceSummaryDTO
            {
                Expenses = expenses,
                SpotSales = spotSales,
                ReceivablesPayments = receivablePayments,
                TotalIncome = totalIncome,
                Balance = totalIncome - expenses
            };
        }
    }
}




