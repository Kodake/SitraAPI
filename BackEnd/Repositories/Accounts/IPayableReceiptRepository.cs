using BackEnd.Entities.Accounts;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Accounts
{
    public interface IPayableReceiptRepository : IRepositoryBase<PayableReceipt>
    {
        new Task<PayableReceipt?> GetByIdAsync(long id);
    }
}
