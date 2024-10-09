using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.Repositories.Warehouses
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetById(int id);
        Task<List<Warehouse>> GetAll(PaginationDTO paginationDTO);
        Task<List<Warehouse>> GetAll(StatusType status);
        Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(int id);
        Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(int id);
        Task<int> Create(Warehouse warehouse);
        Task Update(Warehouse warehouse);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
