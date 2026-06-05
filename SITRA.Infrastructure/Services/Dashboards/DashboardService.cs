using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Dashboards;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDTO> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            var totalWarehouses = await _context.Warehouses
                .CountAsync(x => x.IsActive, cancellationToken);

            var totalContactsAdded = await _context.Contacts
                .CountAsync(x => x.IsActive, cancellationToken);

            var last24HoursSaleInvoices = await _context.Invoices
                .Where(x => x.InvoiceType == InvoiceType.Sale && (x.SpotPayment != null || x.Receivable != null) && x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last24Hours))
                .CountAsync(cancellationToken);

            var totalProductsInStock = await _context.Stocks
                .CountAsync(x => x.Qty > 0, cancellationToken);

            var last30DaysSales = await _context.Invoices
                .Where(x => x.InvoiceType == InvoiceType.Sale && (x.SpotPayment != null || x.Receivable != null) && x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(x => x.TotalPrice, cancellationToken);

            var last30DaysPurchases = await _context.Invoices
                .Where(i => i.InvoiceType == InvoiceType.Purchase && (i.SpotPayable != null || i.Payable != null) && i.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(i => i.TotalPrice, cancellationToken);

            return new DashboardDTO
            {
                TotalWarehouses = totalWarehouses,
                TotalContactsAdded = totalContactsAdded,
                Last24HoursSaleInvoices = last24HoursSaleInvoices,
                TotalProductsInStock = totalProductsInStock,
                Last30DaysSales = last30DaysSales,
                Last30DaysPurchases = last30DaysPurchases
            };
        }

        public async Task<StockAnalysisDTO> GetStockAnalysisAsync(int year, CancellationToken cancellationToken = default)
        {
            var months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var purchasedStock = new List<double>(new double[12]);
            var soldStock = new List<double>(new double[12]);

            var invoices = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .Where(i => i.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            foreach (var inv in invoices)
            {
                int monthIndex = inv.CreatedAt.Month - 1;
                double totalQty = inv.InvoiceItems.Sum(x => x.Qty);

                if (inv.InvoiceType == InvoiceType.Purchase)
                    purchasedStock[monthIndex] += totalQty;
                else if (inv.InvoiceType == InvoiceType.Sale)
                    soldStock[monthIndex] += totalQty;
            }

            return new StockAnalysisDTO
            {
                Months = months,
                PurchasedStock = purchasedStock,
                SoldStock = soldStock
            };
        }

        public async Task<OverallReportDTO> GetOverallReportAsync(CancellationToken cancellationToken = default)
        {
            var sales = await _context.Invoices
                .Where(i => i.InvoiceType == InvoiceType.Sale)
                .SumAsync(i => i.TotalPrice, cancellationToken);

            var purchases = await _context.Invoices
                .Where(i => i.InvoiceType == InvoiceType.Purchase)
                .SumAsync(i => i.TotalPrice, cancellationToken);

            var expenses = await _context.Expenses
                .SumAsync(e => e.Amount, cancellationToken);

            return new OverallReportDTO
            {
                Sales = sales,
                Purchase = purchases,
                Expense = expenses,
                GrossProfit = sales - purchases
            };
        }

        public async Task<CashFlowReportDTO> GetCashFlowReportAsync(int year, CancellationToken cancellationToken = default)
        {
            var months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var incomes = new List<double>(new double[12]);
            var expenses = new List<double>(new double[12]);

            var invoices = await _context.Invoices
                .Where(i => i.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            var dbExpenses = await _context.Expenses
                .Where(e => e.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            foreach (var inv in invoices)
            {
                int monthIndex = inv.CreatedAt.Month - 1;
                if (inv.InvoiceType == InvoiceType.Sale)
                    incomes[monthIndex] += inv.TotalPrice;
                else if (inv.InvoiceType == InvoiceType.Purchase)
                    expenses[monthIndex] += inv.TotalPrice;
            }

            foreach (var exp in dbExpenses)
            {
                int monthIndex = exp.CreatedAt.Month - 1;
                expenses[monthIndex] += exp.Amount;
            }

            return new CashFlowReportDTO
            {
                Months = months,
                Income = incomes,
                Expenses = expenses
            };
        }

        public async Task<List<TopSaleDTO>> GetTopSalesAsync(int month, int year, CancellationToken cancellationToken = default)
        {
            var topSales = await _context.InvoiceItems
                .Include(ii => ii.Invoice)
                .Include(ii => ii.Item)
                .Where(ii => ii.Invoice.InvoiceType == InvoiceType.Sale && ii.Invoice.CreatedAt.Month == month && ii.Invoice.CreatedAt.Year == year)
                .GroupBy(ii => new { ii.ItemId, ii.Item.Name })
                .Select(g => new TopSaleDTO
                {
                    Name = g.Key.Name,
                    Quantity = (int)g.Sum(x => x.Qty),
                    Batch = "[B-" + g.Key.ItemId.ToString().PadLeft(4, '0') + "]"
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < topSales.Count; i++)
            {
                topSales[i].Sl = (i + 1).ToString().PadLeft(2, '0');
            }

            return topSales;
        }
    }
}
