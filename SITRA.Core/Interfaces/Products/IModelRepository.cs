using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IModelRepository : IRepositoryBase<Model>
    {
        new Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO);
        Task<bool> Exists(Guid id, string name);
        new Task<Model?> GetByIdAsync(Guid id);
        Task<List<Model>> GetByBrandId(Guid brandId);
    }
}
