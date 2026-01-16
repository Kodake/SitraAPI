using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface IStockRepository : IRepositoryBase<Stock>
    {
        new Task<List<Stock>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Stock?> GetByItemIdAndWarehouseId(long itemId, long warehouseId);
        Task<double> TotalProductsInStockAsync();
        Task<List<Stock>> GetAllByWarehouseIdsAsync(List<long> warehouseIds, PaginationDTO paginationDTO);
        Task<List<Stock>> GetStockByWarehouseIdsAsync(List<long> warehouseIds);
    }
}
