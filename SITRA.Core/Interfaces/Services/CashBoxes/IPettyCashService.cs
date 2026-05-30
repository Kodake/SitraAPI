using SITRA.Core.DTOs.CashBoxes;

namespace SITRA.Core.Interfaces.Services.CashBoxes
{
    public interface IPettyCashService
    {
        Task<Dictionary<object, double>> GetPaymentMethodSumsAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<List<PaymentMethodAmountListDTO>> GetPaymentMethodAmountListAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<double> GetTotalWithdrawalsAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
    }
}
