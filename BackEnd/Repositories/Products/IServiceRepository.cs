using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface IServiceRepository
    {
        Task<Item?> GetById(long id);
        Task<List<Item>> GetAll(PaginationDTO paginationDTO);
        Task<List<Item>> GetAll(StatusType status);
        Task<long> Create(Item service);
        Task Update(Item service);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string name);
    }
}
