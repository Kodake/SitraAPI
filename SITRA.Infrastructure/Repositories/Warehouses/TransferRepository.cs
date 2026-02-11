using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Warehouses;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Warehouses
{
    public class TransferRepository : RepositoryBase<Transfer>, ITransferRepository
    {
        private new readonly HttpContext httpContext;
        public TransferRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<Transfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Transfers
                .Include(t => t.Product)
                .Include(t => t.DestinationWarehouse)
                .AsNoTracking()
                .Where(t => t.Product!.ItemType == ItemType.Product)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async new Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Transfers
                .Include(t => t.Product)
                .Include(t => t.Stock)
                    .ThenInclude(s => s!.Warehouse)
                .Include(t => t.DestinationWarehouse)
                .Where(t => t.Product!.ItemType == ItemType.Product)
                .OrderByDescending(t => t.Status == TransferStatus.Transit)
                .ThenBy(t => t.Id);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}




