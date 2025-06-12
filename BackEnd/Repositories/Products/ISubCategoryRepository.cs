using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface ISubCategoryRepository : IRepositoryBase<SubCategory>
    {
        new Task<List<SubCategory>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<SubCategory?> GetByIdAsync(long id);
        Task<List<SubCategory>> GetByCategoryId(long categoryId);
        Task<bool> Exists(long id, string name);
    }
}
