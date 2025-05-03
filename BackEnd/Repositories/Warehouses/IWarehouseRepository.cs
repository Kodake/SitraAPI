using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Warehouses
{
    public interface IWarehouseRepository : IRepositoryBase<Warehouse>
    {
        Task<List<Warehouse>> GetAll(StatusType status);
        Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(long id);
        Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(long id);
        Task<bool> Exists(long id, string code);
        Task<int> TotalWarehousesAddedAsync();
    }
}
