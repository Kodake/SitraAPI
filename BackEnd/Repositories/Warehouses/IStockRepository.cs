using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Repositories.Warehouses
{
    public interface IStockRepository
    {
        Task<Stock?> GetById(long id);
        Task<List<Stock>> GetAll(PaginationDTO paginationDTO);
        Task<bool> Create(Stock stock);
        Task Update(Stock stock);
        Task<Stock?> GetByItemIdAndWarehouseId(long itemId, long warehouseId);
    }
}
