using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<List<Category>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
