using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface IProductRepository
    {
        Task<Product?> GetById(int id);
        Task<List<Product>> GetAll(PaginationDTO paginationDTO);
        Task<List<Product>> GetAll(StatusType status);
        Task<int> Create(Product product);
        Task Update(Product product);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
    }
}
