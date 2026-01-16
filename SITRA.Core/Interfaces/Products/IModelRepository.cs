using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface IModelRepository : IRepositoryBase<Model>
    {
        new Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO);
        Task<bool> Exists(long id, string name);
        new Task<Model?> GetByIdAsync(long id);
        Task<List<Model>> GetByBrandId(long brandId);
    }
}
