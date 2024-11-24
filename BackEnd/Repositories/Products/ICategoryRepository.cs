using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAll(PaginationDTO paginationDTO);
        Task<List<Category>> GetAll(StatusType status);
        Task<Category?> GetById(long id);
        Task<long> Create(Category category);
        Task Update(Category category);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string code);
    }
}
