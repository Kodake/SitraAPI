using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface IWarehouseRepository : IRepositoryBase<Warehouse>
    {
        Task<List<Warehouse>> GetAll(StatusType status);
        Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(Guid id);
        Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(Guid id);
        Task<bool> Exists(Guid id, string code);
        Task<int> TotalWarehousesAddedAsync();
    }
}
