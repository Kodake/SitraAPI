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
using SITRA.Core.Helpers;
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
                .WhereValidSaleInvoice()
                .Where(x => x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last24Hours))
                .CountAsync(cancellationToken);

            var totalProductsInStock = await _context.Stocks
                .CountAsync(x => x.Qty > 0, cancellationToken);

            var last30DaysSales = await _context.Invoices
                .WhereValidSaleInvoice()
                .Where(x => x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(x => x.TotalPrice, cancellationToken);

            var last30DaysPurchases = await _context.Invoices
                .WhereValidPurchaseInvoice()
                .Where(i => i.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
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

            var saleInvoices = await _context.Invoices
                .WhereValidSaleInvoice()
                .Include(i => i.InvoiceItems)
                .Where(i => i.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            var purchaseInvoices = await _context.Invoices
                .WhereValidPurchaseInvoice()
                .Include(i => i.InvoiceItems)
                .Where(i => i.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            foreach (var inv in saleInvoices)
            {
                int monthIndex = inv.CreatedAt.Month - 1;
                soldStock[monthIndex] += inv.InvoiceItems.Sum(x => x.Qty);
            }

            foreach (var inv in purchaseInvoices)
            {
                int monthIndex = inv.CreatedAt.Month - 1;
                purchasedStock[monthIndex] += inv.InvoiceItems.Sum(x => x.Qty);
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
                .WhereValidSaleInvoice()
                .SumAsync(i => i.TotalPrice, cancellationToken);

            var purchases = await _context.Invoices
                .WhereValidPurchaseInvoice()
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

            var saleInvoices = await _context.Invoices
                .WhereValidSaleInvoice()
                .Where(i => i.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            var purchaseInvoices = await _context.Invoices
                .WhereValidPurchaseInvoice()
                .Where(i => i.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            var dbExpenses = await _context.Expenses
                .Where(e => e.CreatedAt.Year == year)
                .ToListAsync(cancellationToken);

            foreach (var inv in saleInvoices)
            {
                int monthIndex = inv.CreatedAt.Month - 1;
                incomes[monthIndex] += inv.TotalPrice;
            }

            foreach (var inv in purchaseInvoices)
            {
                int monthIndex = inv.CreatedAt.Month - 1;
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
                .Where(ii => ii.Invoice.InvoiceType == InvoiceType.Sale
                    && ii.Invoice.Status != InvoiceStatus.Rejected
                    && ii.Invoice.Status != InvoiceStatus.Failed
                    && ii.Invoice.Status != InvoiceStatus.Cancelled
                    && ii.Invoice.DgiiStatus != "Rechazado"
                    && ii.Invoice.DgiiStatus != "Error"
                    && (ii.Invoice.SpotPayment != null || ii.Invoice.Receivable != null)
                    && ii.Invoice.CreatedAt.Month == month
                    && ii.Invoice.CreatedAt.Year == year)
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

        public async Task<List<TopCustomerDTO>> GetTopCustomersAsync(int month, int year, CancellationToken cancellationToken = default)
        {
            var topCustomers = await _context.Invoices
                .AsNoTracking()
                .WhereValidSaleInvoice()
                .Where(i => i.CreatedAt.Month == month && i.CreatedAt.Year == year && i.BuyerId != null)
                .GroupBy(i => new { i.BuyerId, i.Buyer!.Name, i.Buyer.Phone })
                .Select(g => new TopCustomerDTO
                {
                    Name = g.Key.Name,
                    Phone = g.Key.Phone ?? "",
                    Amount = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < topCustomers.Count; i++)
            {
                topCustomers[i].Sl = (i + 1).ToString().PadLeft(2, '0');
            }

            return topCustomers;
        }
    }
}

