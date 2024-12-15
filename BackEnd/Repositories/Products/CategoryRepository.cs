using BackEnd.Context;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor) { }

        public async Task<List<Category>> GetAll(StatusType status)
        {
            IQueryable<Category> query = context.Categories.AsQueryable();

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
            return await context.Categories.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
