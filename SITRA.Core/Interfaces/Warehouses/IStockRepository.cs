using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface IStockRepository : IRepositoryBase<Stock>
    {
        new Task<List<Stock>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Stock?> GetByItemIdAndWarehouseId(Guid itemId, Guid warehouseId);
        Task<double> TotalProductsInStockAsync();
        Task<List<Stock>> GetAllByWarehouseIdsAsync(List<Guid> warehouseIds, PaginationDTO paginationDTO);
        Task<List<Stock>> GetStockByWarehouseIdsAsync(List<Guid> warehouseIds);
    }
}
