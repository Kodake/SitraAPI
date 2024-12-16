using BackEnd.Entities.Accounts;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Accounts
{
    public interface IPaymentReceiptRepository : IRepositoryBase<PaymentReceipt>
    {
        new Task<PaymentReceipt?> GetByIdAsync(long id);
    }
}
