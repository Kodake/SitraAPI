using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IServiceService
    {
        Task<List<Item>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<List<Item>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
