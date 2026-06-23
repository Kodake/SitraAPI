using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Warehouses;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Warehouses
{
    public class TransferService : ITransferService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public TransferService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Transfers.AsNoTracking()
                .Include(t => t.Product)
                .Include(t => t.Stock)
                    .ThenInclude(s => s!.Warehouse)
                .Include(t => t.DestinationWarehouse)
                .Where(t => t.Product!.ItemType == ItemType.Product);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(t => 
                    (t.Product != null && t.Product.Name != null && t.Product.Name.ToLower().Contains(searchTerm)) ||
                    (t.Comment != null && t.Comment.ToLower().Contains(searchTerm))
                );
            }

            queryable = queryable.OrderByDescending(t => t.Status == TransferStatus.Transit)
                .ThenBy(t => t.Id);

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
