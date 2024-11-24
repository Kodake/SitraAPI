using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;

namespace BackEnd.Repositories.Warehouses
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetById(long id);
        Task<List<Warehouse>> GetAll(PaginationDTO paginationDTO);
        Task<List<Warehouse>> GetAll(StatusType status);
        Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(long id);
        Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(long id);
        Task<long> Create(Warehouse warehouse);
        Task Update(Warehouse warehouse);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string code);
    }
}
