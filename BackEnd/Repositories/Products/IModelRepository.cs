using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IModelRepository : IRepositoryBase<Model>
    {
        new Task<List<Model>> GetAllAsync(PaginationDTO paginationDTO);
        Task<bool> Exists(long id, string name);
        new Task<Model?> GetByIdAsync(long id);
        Task<List<Model>> GetByBrandId(long brandId);
    }
}
