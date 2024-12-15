using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Warehouses
{
    public class TransferRepository : RepositoryBase<Transfer>, ITransferRepository
    {
        private new readonly HttpContext httpContext;
        public TransferRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<Transfer?> GetByIdAsync(long id)
        {
            return await context.Transfers
                .Include(t => t.Product)
                .Include(t => t.DestinationWarehouse)
                .AsNoTracking()
                .Where(t => t.Product!.ItemType == ItemType.Product)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async new Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO)
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
                .ToListAsync();
        }
    }
}
