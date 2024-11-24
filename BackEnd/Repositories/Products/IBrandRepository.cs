using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface IBrandRepository
    {
        Task<List<Brand>> GetAll(PaginationDTO paginationDTO);
        Task<List<Brand>> GetAll(StatusType status);
        Task<Brand?> GetById(long id);
        Task<long> Create(Brand brand);
        Task Update(Brand brand);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string code);
    }
}
