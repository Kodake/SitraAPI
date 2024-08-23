using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface IServiceRepository
    {
        Task<Item?> GetById(int id);
        Task<List<Item>> GetAll(PaginationDTO paginationDTO);
        Task<List<Item>> GetAll(StatusType status);
        Task<int> Create(Item service);
        Task Update(Item service);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
    }
}
