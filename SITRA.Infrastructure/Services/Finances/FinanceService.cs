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
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Finances
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDbContext _context;

        public FinanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BalanceSummaryDTO> GetSaleBalanceAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var expenses = await _context.Expenses
                .Where(x => x.Date >= from && x.Date <= to && x.IsActive)
                .SumAsync(x => x.Amount, cancellationToken);

            var spotSales = await _context.SpotPayments
                .Include(sp => sp.Invoice)
                .Where(sp => sp.Invoice.CreatedAt >= from && sp.Invoice.CreatedAt <= to && sp.IsActive)
                .SumAsync(sp => sp.Invoice.TotalPrice, cancellationToken);

            var receivablePayments = await _context.PaymentReceipts
                .Where(pr => pr.CreatedAt >= from && pr.CreatedAt <= to && pr.IsActive)
                .SumAsync(pr => pr.Amount, cancellationToken);

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

        public async Task<PurchaseBalanceSummaryDTO> GetPurchaseBalanceAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var spotPurchases = await _context.SpotPayments
                .Include(sp => sp.Invoice)
                .Where(sp => sp.Invoice.InvoiceType == InvoiceType.Purchase && sp.Invoice.CreatedAt >= from && sp.Invoice.CreatedAt <= to && sp.IsActive)
                .SumAsync(sp => sp.Invoice.TotalPrice, cancellationToken);

            var payablesPayments = await _context.PayableReceipts
                .Where(pr => pr.CreatedAt >= from && pr.CreatedAt <= to && pr.IsActive)
                .SumAsync(pr => pr.Amount, cancellationToken);

            var totalOutflow = spotPurchases + payablesPayments;

            return new PurchaseBalanceSummaryDTO
            {
                SpotPurchases = spotPurchases,
                PayablesPayments = payablesPayments,
                TotalOutflow = totalOutflow
            };
        }
    }
}
