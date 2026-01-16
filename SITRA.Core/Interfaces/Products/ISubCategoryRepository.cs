using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Products
{
    public interface ISubCategoryRepository : IRepositoryBase<SubCategory>
    {
        new Task<List<SubCategory>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<SubCategory?> GetByIdAsync(long id);
        Task<List<SubCategory>> GetByCategoryId(long categoryId);
        Task<bool> Exists(long id, string name);
    }
}
