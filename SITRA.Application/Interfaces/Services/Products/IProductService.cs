using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IProductService
    {
        Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Item>> GetStockProductsAsync(CancellationToken cancellationToken = default);
        Task<List<Item>> GetPaginatedAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
