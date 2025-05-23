using BackEnd.DTOs;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Entities.CashBoxes;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.CashBoxes
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
