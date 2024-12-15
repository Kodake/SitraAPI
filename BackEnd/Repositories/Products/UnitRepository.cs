using BackEnd.Context;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class UnitRepository : RepositoryBase<Unit>, IUnitRepository
    {
        public UnitRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Unit>> GetAll(StatusType status)
        {
            IQueryable<Unit> query = context.Units.AsQueryable();

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

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Units.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
