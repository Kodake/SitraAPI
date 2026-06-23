using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Products
{
    public class ModelService : IModelService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public ModelService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Models.AsNoTracking()
                .OrderBy(x => x.Name);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            var subCategories = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Brand)
                .ToListAsync(cancellationToken);

            return subCategories;
        }
    }
}
