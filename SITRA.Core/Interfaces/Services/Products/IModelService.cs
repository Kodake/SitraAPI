using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;

namespace SITRA.Core.Interfaces.Services.Products
{
    public interface IModelService
    {
        Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
