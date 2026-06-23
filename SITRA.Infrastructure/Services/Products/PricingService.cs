using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Products
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public PricingService(IUnitOfWork unitOfWork, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<IEnumerable<Pricing>> GetUnregisteredPricingsForItemAsync(Guid itemId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.PricingRepository.GetAllAsync(p => !p.PricingItems.Any(pi => pi.ItemId == itemId), cancellationToken);
        }

        public async Task<bool> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var pricing = await _unitOfWork.PricingRepository.GetByIdAsync(id, cancellationToken);
            if (pricing == null) return false;

            var defaults = await _unitOfWork.PricingRepository.GetAllAsync(x => x.IsDefault && x.Id != id, cancellationToken);

            foreach (var p in defaults)
            {
                p.IsDefault = false;
                _unitOfWork.PricingRepository.Update(p);
            }

            pricing.IsDefault = true;
            _unitOfWork.PricingRepository.Update(pricing);
            
            return true;
        }

        public async Task<List<Pricing>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Pricings.AsNoTracking();

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
