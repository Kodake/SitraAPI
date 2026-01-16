using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
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
