using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Accounts
{
    public interface IPayableReceiptRepository : IRepositoryBase<PayableReceipt>
    {
        new Task<PayableReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
