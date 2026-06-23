using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.Core.Interfaces.Services.Warehouses
{
    public interface IStockService
    {
        Task<List<Stock>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
