using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.Products
{
    public interface IServiceRepository : IRepositoryBase<Item>
    {
        new Task<Item?> GetByIdAsync(long id);
        new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO);
        Task<List<Item>> GetAll(StatusType status);
        new Task ActivateAsync(long id);
        new Task DeactivateAsync(long id);
        new Task<bool> ExistsAsync(long id);
        Task<bool> Exists(long id, string name);
    }
}
