using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.CashBoxes
{
    public interface IPettyCashMovementRepository : IRepositoryBase<PettyCashMovement>
    {
        Task<List<PettyCashMovement>> GetAllAsync(PaginationDTO paginationDTO, Guid pettyCashId);
        Task<List<PettyCashMovement>> GetAllWithdrawalsAsync(PaginationDTO paginationDTO, Guid pettyCashId);
        Task<double> GetTotalWithdrawalsAsync(Guid pettyCashId);
        Task<double> GetTotalAmountPaidAsync(Guid pettyCashId);
        Task<Dictionary<object, double>> GetPaymentMethodSumsAsync(Guid pettyCashId);
        Task<List<PaymentMethodAmountListDTO>> GetPaymentMethodAmountListAsync(Guid pettyCashId);
    }
}
