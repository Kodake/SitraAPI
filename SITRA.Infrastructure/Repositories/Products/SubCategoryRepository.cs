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
    public class SubCategoryRepository : RepositoryBase<SubCategory>, ISubCategoryRepository
    {
        private new readonly HttpContext httpContext;
        public SubCategoryRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<SubCategory>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.SubCategories
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var subCategories = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);

            return subCategories;
        }

        public async new Task<List<SubCategory>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.SubCategories.AsNoTracking()
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var subCategories = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Category)
                .ToListAsync(cancellationToken);

            return subCategories;
        }

        public async new Task<SubCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.SubCategories
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public Task<List<SubCategory>> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken = default) =>
            context.SubCategories
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync(cancellationToken);

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.SubCategories.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }
    }
}




