using BackEnd.Context;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class BrandRepository : RepositoryBase<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Brand>> GetAll(StatusType status)
        {
            IQueryable<Brand> query = context.Brands.AsQueryable();

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
            return await context.Brands.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
