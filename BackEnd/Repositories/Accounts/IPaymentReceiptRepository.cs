using BackEnd.DTOs;
using BackEnd.Entities.Accounts;

namespace BackEnd.Repositories.Accounts
{
    public interface IPaymentReceiptRepository
    {
        Task<List<PaymentReceipt>> GetAll(PaginationDTO paginationDTO, long receivableId);
        Task<long> Create(PaymentReceipt paymentReceipt);
        Task<PaymentReceipt?> GetByIdAndReceivableId(long id, long receivableId);
    }
}
