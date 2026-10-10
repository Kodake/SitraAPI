using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;

using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface IStockRepository : IRepositoryBase<Stock>
    {
        Task<Stock?> GetByItemIdAndWarehouseId(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default);
        Task<List<Stock>> GetAllByWarehouseIdsAsync(List<Guid> warehouseIds, PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Stock>> GetStockByWarehouseIdsAsync(List<Guid> warehouseIds, CancellationToken cancellationToken = default);
    }
}
