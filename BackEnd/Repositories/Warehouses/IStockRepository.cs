using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Warehouses
{
    public interface IStockRepository : IRepositoryBase<Stock>
    {
        new Task<List<Stock>> GetAllAsync(PaginationDTO paginationDTO);
        Task<Stock?> GetByItemIdAndWarehouseId(long itemId, long warehouseId);
        Task<double> TotalProductsInStockAsync();
    }
}
