using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.DTOs.Invoices.SaleInvoices;

namespace SITRA.Core.Interfaces.Services.Finances
{
    public interface IFinanceService
    {
        Task<BalanceSummaryDTO> GetSaleBalanceAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
        Task<PurchaseBalanceSummaryDTO> GetPurchaseBalanceAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
    }
}
