using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<List<Brand>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
