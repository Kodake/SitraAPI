using BackEnd.DTOs;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface ICategoryRepository
    {
        Task<Category?> GetById(int id);
        Task<List<Category>> GetAll(PaginationDTO paginationDTO);
        Task<int> Create(Category category);
        Task Update(Category category);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
