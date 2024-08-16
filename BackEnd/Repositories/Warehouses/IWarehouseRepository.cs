using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Repositories.Warehouses
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetById(int id);
        Task<List<Warehouse>> GetAll(PaginationDTO paginationDTO);
        Task<int> Create(Warehouse warehouse);
        Task Update(Warehouse warehouse);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
