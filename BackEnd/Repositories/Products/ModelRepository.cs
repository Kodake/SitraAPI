using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ModelRepository : RepositoryBase<Model>, IModelRepository
    {
        private new readonly HttpContext httpContext;
        public ModelRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Models
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var subCategories = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Brand)
                .ToListAsync();

            return subCategories;
        }

        public async new Task<Model?> GetByIdAsync(long id)
        {
            return await context.Models
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Models.AnyAsync(x => x.Id != id && x.Name == name);
        }

        public Task<List<Model>> GetByBrandId(long brandId) =>
            context.Models
                .Where(x => x.BrandId == brandId)
                .ToListAsync();
    }
}