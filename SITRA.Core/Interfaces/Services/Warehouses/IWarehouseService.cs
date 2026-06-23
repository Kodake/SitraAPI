using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.Core.Interfaces.Services.Warehouses
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
