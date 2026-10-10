using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface IWarehouseRepository : IRepositoryBase<Warehouse>
    {

        Task<List<Warehouse>> GetAll(StatusType status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Warehouse>> GetStockWarehouseByProductId(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Warehouse>> GetUnregisteredWarehouseById(Guid id, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string code, CancellationToken cancellationToken = default);
        Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default);
    }
}
