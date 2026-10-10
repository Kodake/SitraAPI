using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.CashBoxes
{
    public interface IPettyCashRepository : IRepositoryBase<PettyCash>
    {
        Task<PettyCash?> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> ExistsOpenAsync(string userId, CancellationToken cancellationToken = default);
        Task<PettyCash?> GetOpenPettyCashAsync(CancellationToken cancellationToken = default);
        Task<PettyCash?> GetOpenPettyCashByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
