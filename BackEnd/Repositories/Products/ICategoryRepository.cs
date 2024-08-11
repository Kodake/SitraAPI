using BackEnd.DTOs;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll(PaginationDTO paginationDTO);
        Task<List<Category>> GetAll(bool isDeleted);
        Task<Category?> GetById(int id);
        Task<int> Create(Category category);
        Task Update(Category category);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
