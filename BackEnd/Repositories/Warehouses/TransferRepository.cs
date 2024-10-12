using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Warehouses
{
    public class TransferRepository : ITransferRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public TransferRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<Transfer?> GetById(int id)
        {
            return await context.Transfers
                .Include(t => t.Product)
                .Include(t => t.Stock)
                    .ThenInclude(s => s.Warehouse)
                .Include(t => t.DestinationWarehouse)
                .Where(t => t.Product.ItemType == ItemType.Product)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Transfer>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Transfers
                .Include(t => t.Product)
                .Include(t => t.Stock)
                    .ThenInclude(s => s.Warehouse)
                .Include(t => t.DestinationWarehouse)
                .Where(t => t.Product.ItemType == ItemType.Product)
                .OrderByDescending(t => t.Status == TransferStatus.Transit)
                .ThenBy(t => t.Id);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Paginate(paginationDTO)
                .ToListAsync();
        }


        public async Task<int> Create(Transfer transfer)
        {
            context.Add(transfer);
            await context.SaveChangesAsync();
            return transfer.Id;
        }

        public async Task Update(Transfer transfer)
        {
            // Check if the transfer is already being tracked
            var existingTransfer = await context.Transfers.FindAsync(transfer.Id);
            if (existingTransfer != null)
            {
                // If it exists, update the properties
                existingTransfer.Status = transfer.Status; // or any other properties you need to update
                context.Entry(existingTransfer).State = EntityState.Modified; // mark as modified
            }
            else
            {
                // Otherwise, add it as a new entity (this should be avoided if updating)
                context.Transfers.Update(transfer);
            }

            await context.SaveChangesAsync();
        }
    }
}
