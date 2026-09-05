using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface ISubCategoryService
    {
        Task<List<SubCategory>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
