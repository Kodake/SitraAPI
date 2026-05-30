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
                .Where(x => x.InvoiceType == InvoiceType.Sale && x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last24Hours))
                .CountAsync(cancellationToken);

            var totalProductsInStock = await _context.Stocks
                .CountAsync(x => x.Qty > 0, cancellationToken);

            var last30DaysSales = await _context.Invoices
                .Where(x => x.InvoiceType == InvoiceType.Sale && x.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
                .SumAsync(x => x.TotalPrice, cancellationToken);

            var last30DaysPurchases = await _context.Invoices
                .Where(i => i.InvoiceType == InvoiceType.Purchase && i.CreatedAt >= DateTimeOffset.UtcNow.AddDays((double)TimeElapsed.Last30Days))
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
    }
}
