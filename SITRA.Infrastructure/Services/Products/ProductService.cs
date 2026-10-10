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
using System.Linq;

namespace SITRA.Infrastructure.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public ProductService(
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Item, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive && u.ItemType == ItemType.Product,
                StatusType.Deactivated => u => !u.IsActive && u.ItemType == ItemType.Product,
                _ => u => u.ItemType == ItemType.Product
            };

            var items = await _unitOfWork.ProductRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }

        public async Task<IEnumerable<Item>> GetStockProductsAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.ProductRepository.GetAllAsync(p => p.ItemType == ItemType.Product && p.Stocks.Any(x => x.Item.ItemType == ItemType.Product), cancellationToken);
            return items.Select(p => new Item
            {
                Id = p.Id,
                Name = p.Name,
                Reference = p.Reference,
                Barcode = p.Barcode,
            });
        }

        public async Task<List<Item>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Items.AsNoTracking()
                .Where(x => x.ItemType == ItemType.Product);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    x.Name.ToLower().Contains(searchTerm) || 
                    (x.Reference != null && x.Reference.ToLower().Contains(searchTerm)) || 
                    (x.Barcode != null && x.Barcode.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderBy(x => x.Name);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .Paginate(paginationDTO)
                .Include(p => p.PricingItems)
                .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                .ThenInclude(pt => pt.Tax)
                .ToListAsync(cancellationToken);
        }
    }
}
