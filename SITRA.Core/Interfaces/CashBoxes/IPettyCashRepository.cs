using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.CashBoxes
{
    public interface IPettyCashRepository : IRepositoryBase<PettyCash>
    {
        new Task<PettyCash> CreateAsync(PettyCash pettyCash);
        Task<PettyCash?> GetAllByUserIdAsync(string userId);
        Task<bool> ExistsOpenAsync(string userId);
        Task<PettyCash?> GetOpenPettyCashAsync();
        Task<PettyCash?> GetOpenPettyCashByUserIdAsync(string userId);
    }
}
