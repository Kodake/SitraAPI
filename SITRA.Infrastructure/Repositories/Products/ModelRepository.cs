using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Products
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

        public async new Task<Model?> GetByIdAsync(Guid id)
        {
            return await context.Models
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> Exists(Guid id, string name)
        {
            return await context.Models.AnyAsync(x => x.Id != id && x.Name == name);
        }

        public Task<List<Model>> GetByBrandId(Guid brandId) =>
            context.Models
                .Where(x => x.BrandId == brandId)
                .ToListAsync();
    }
}




