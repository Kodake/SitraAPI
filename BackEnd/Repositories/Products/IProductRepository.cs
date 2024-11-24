using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products;

public interface IProductRepository
{
    Task<Item?> GetById(long id);
    Task<List<Item>> GetAll(PaginationDTO paginationDTO);
    Task<List<Item>> GetAll(StatusType status);
    Task<IEnumerable<Item>> GetAll();
    Task<long> Create(Item product);
    Task Update(Item product);
    Task Activate(long id);
    Task Deactivate(long id);
    Task<bool> Exists(long id);
    Task<bool> Exists(long id, string name);
}
