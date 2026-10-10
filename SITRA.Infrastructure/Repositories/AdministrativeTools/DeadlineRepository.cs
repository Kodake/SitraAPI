using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.AdministrativeTools
{
    public class DeadlineRepository : RepositoryBase<Deadline>, IDeadlineRepository
    {
        public DeadlineRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }



        public async Task<bool> Exists(Guid id, long days, CancellationToken cancellationToken = default)
        {
            return await context.Deadlines.AnyAsync(x => x.Id != id && x.Days == days, cancellationToken);
        }
    }
}






