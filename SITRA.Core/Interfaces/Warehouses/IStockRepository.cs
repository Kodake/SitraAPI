using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface IStockRepository : IRepositoryBase<Stock>
    {
        new Task<List<Stock>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Stock>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<Stock?> GetByItemIdAndWarehouseId(Guid itemId, Guid warehouseId, CancellationToken cancellationToken = default);
        Task<double> TotalProductsInStockAsync(CancellationToken cancellationToken = default);
        Task<List<Stock>> GetAllByWarehouseIdsAsync(List<Guid> warehouseIds, PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Stock>> GetStockByWarehouseIdsAsync(List<Guid> warehouseIds, CancellationToken cancellationToken = default);
    }
}
