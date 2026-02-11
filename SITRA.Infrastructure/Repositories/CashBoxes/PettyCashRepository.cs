using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.CashBoxes;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.CashBoxes
{
    public class PettyCashRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PettyCash>(context, httpContextAccessor), IPettyCashRepository
    {
        public async Task<bool> ExistsOpenAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await context.PettyCashes.AnyAsync(pc => pc.UserId == userId && pc.Status == PettyCashStatus.Open, cancellationToken);
        }

        public async Task<PettyCash?> GetOpenPettyCashAsync(CancellationToken cancellationToken = default)
        {
            return await context.PettyCashes
                .Include(pc => pc.User)
                .FirstOrDefaultAsync(pc => pc.Status == PettyCashStatus.Open, cancellationToken);
        }

        public async Task<PettyCash?> GetOpenPettyCashByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await context.PettyCashes
                .Include(pc => pc.User)
                .FirstOrDefaultAsync(pc => pc.UserId == userId && pc.Status == PettyCashStatus.Open, cancellationToken);
        }

        public async Task<PettyCash?> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await context.PettyCashes
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == PettyCashStatus.Open, cancellationToken);
        }
    }
}





