using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.CashBoxes
{
    public interface IPettyCashMovementRepository : IRepositoryBase<PettyCashMovement>
    {
        Task<List<PettyCashMovement>> GetAllAsync(PaginationDTO paginationDTO, long pettyCashId);
        Task<List<PettyCashMovement>> GetAllWithdrawalsAsync(PaginationDTO paginationDTO, long pettyCashId);
        Task<double> GetTotalWithdrawalsAsync(long pettyCashId);
        Task<double> GetTotalAmountPaidAsync(long pettyCashId);
        Task<Dictionary<object, double>> GetPaymentMethodSumsAsync(long pettyCashId);
        Task<List<PaymentMethodAmountListDTO>> GetPaymentMethodAmountListAsync(long pettyCashId);
    }
}
