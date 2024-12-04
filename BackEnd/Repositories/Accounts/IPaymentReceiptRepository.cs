using BackEnd.DTOs;
using BackEnd.Entities.Accounts;

namespace BackEnd.Repositories.Accounts
{
    public interface IPaymentReceiptRepository
    {
        Task<List<PaymentReceipt>> GetAll(PaginationDTO paginationDTO, long receivableId);
        Task<long> Create(long receivableId, PaymentReceipt paymentReceipt);
        Task<PaymentReceipt?> GetById(long id);
        Task Deactivate(long paymentReceiptId);
    }
}
