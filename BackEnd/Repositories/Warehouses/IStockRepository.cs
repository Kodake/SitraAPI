using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Repositories.Warehouses
{
    public interface IStockRepository
    {
        Task<Stock?> GetById(int productId, int warehouseId);
        Task<List<Stock>> GetAll(PaginationDTO paginationDTO);
        Task<bool> Create(Stock warehouse);
        Task Update(Stock warehouse);
    }
}
