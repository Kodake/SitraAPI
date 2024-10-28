using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Repositories.Warehouses
{
    public interface IStockRepository
    {
        Task<Stock?> GetById(int id);
        Task<List<Stock>> GetAll(PaginationDTO paginationDTO);
        Task<bool> Create(Stock stock);
        Task Update(Stock stock);
        Task<Stock?> GetByItemIdAndWarehouseId(int itemId, int warehouseId);
    }
}
