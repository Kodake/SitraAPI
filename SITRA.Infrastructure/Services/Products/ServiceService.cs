using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace SITRA.Infrastructure.Services.Products
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SITRA.Infrastructure.Persistence.ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;

        public ServiceService(IUnitOfWork unitOfWork, SITRA.Infrastructure.Persistence.ApplicationDbContext context, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Item, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive && u.ItemType == ItemType.Service,
                StatusType.Deactivated => u => !u.IsActive && u.ItemType == ItemType.Service,
                _ => u => u.ItemType == ItemType.Service
            };

            var items = await _unitOfWork.ServiceRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }

        public async Task<List<Item>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Items.AsNoTracking()
                .Where(x => x.ItemType == ItemType.Service);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.Name != null && x.Name.ToLower().Contains(searchTerm)) || 
                    (x.Reference != null && x.Reference.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderBy(x => x.Name);

            var httpContext = _httpContext;

            if (httpContext != null)
            {
                await SITRA.Core.Helpers.HttpContextExtensions.AddPaginationParamsToHeaders(httpContext, queryable);
            }

            var items = await SITRA.Core.Helpers.IQueryableExtensions.Paginate(queryable, paginationDTO)
                .Include(p => p.PricingItems)
                .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                .ThenInclude(pt => pt.Tax)
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
