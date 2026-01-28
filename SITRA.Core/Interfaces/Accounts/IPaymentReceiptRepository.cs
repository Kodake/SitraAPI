using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IPaymentReceiptRepository : IRepositoryBase<PaymentReceipt>
    {
        new Task<PaymentReceipt?> GetByIdAsync(Guid id);
    }
}
