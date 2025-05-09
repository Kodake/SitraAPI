using BackEnd.Context;
using BackEnd.Entities.CashBoxes;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.CashBoxes
{
    public class PettyCashRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PettyCash>(context, httpContextAccessor), IPettyCashRepository
    {
        public async Task<bool> ExistsOpenAsync(string userId)
        {
            return await context.PettyCashes.AnyAsync(pc => pc.UserId == userId && pc.Status == PettyCashStatus.Open);
        }

        public async Task<PettyCash?> GetOpenPettyCashAsync()
        {
            return await context.PettyCashes
                .Include(pc => pc.User)
                .FirstOrDefaultAsync(pc => pc.Status == PettyCashStatus.Open);
        }

        public async Task<PettyCash?> GetOpenPettyCashByUserIdAsync(string userId)
        {
            return await context.PettyCashes
                .Include(pc => pc.User)
                .FirstOrDefaultAsync(pc => pc.UserId == userId && pc.Status == PettyCashStatus.Open);
        }

        public async Task<PettyCash?> GetAllByUserIdAsync(string userId)
        {
            return await context.PettyCashes
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == PettyCashStatus.Open);
        }
    }
}
