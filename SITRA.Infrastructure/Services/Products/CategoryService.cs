using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.Products
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public CategoryService(IUnitOfWork unitOfWork, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Category>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Category, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(filter, cancellationToken);
            return categories.OrderBy(u => u.Id).ToList();
        }

        public async Task<List<Category>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Categories.AsNoTracking();

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => x.Name != null && x.Name.ToLower().Contains(searchTerm));
            }

            queryable = queryable.OrderBy(x => x.Name);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable.Paginate(paginationDTO).ToListAsync(cancellationToken);
        }
    }
}
