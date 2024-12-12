using BackEnd.Context;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BackEnd.Repositories.AdministrativeTools
{
    public class DeadlineRepository : RepositoryBase<Deadline>, IDeadlineRepository
    {
        public DeadlineRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Deadline>> GetAll(StatusType status)
        {
            IQueryable<Deadline> query = context.Deadlines.AsQueryable();

            switch (status)
            {
                case StatusType.Activated:
                    query = query.Where(u => u.IsActive);
                    break;
                case StatusType.Deactivated:
                    query = query.Where(u => !u.IsActive);
                    break;
                case StatusType.All:
                default:
                    break;
            }

            return await query.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<bool> Exists(long id, long days)
        {
            return await context.Deadlines.AnyAsync(x => x.Id != id && x.Days == days);
        }
    }
}
