using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.AdministrativeTools
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

        public async Task<bool> Exists(Guid id, long days)
        {
            return await context.Deadlines.AnyAsync(x => x.Id != id && x.Days == days);
        }
    }
}





