using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface ISubCategoryRepository : IRepositoryBase<SubCategory>
    {
        new Task<List<SubCategory>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<SubCategory?> GetByIdAsync(Guid id);
        Task<List<SubCategory>> GetByCategoryId(Guid categoryId);
        Task<bool> Exists(Guid id, string name);
    }
}
