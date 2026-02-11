using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

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

        public async new Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Models
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var subCategories = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Brand)
                .ToListAsync(cancellationToken);

            return subCategories;
        }

        public async new Task<Model?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Models
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Models.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }

        public Task<List<Model>> GetByBrandId(Guid brandId, CancellationToken cancellationToken = default) =>
            context.Models
                .Where(x => x.BrandId == brandId)
                .ToListAsync(cancellationToken);
    }
}




