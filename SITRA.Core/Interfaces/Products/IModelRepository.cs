using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IModelRepository : IRepositoryBase<Model>
    {
        new Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Model>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default);
        new Task<Model?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Model>> GetByBrandId(Guid brandId, CancellationToken cancellationToken = default);
    }
}
