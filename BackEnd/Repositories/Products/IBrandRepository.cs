using BackEnd.DTOs;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAll(PaginationDTO paginationDTO);
        Task<List<Brand>> GetAll(bool isDeleted);
        Task<Brand?> GetById(int id);
        Task<int> Create(Brand brand);
        Task Update(Brand brand);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string code);
    }
}
