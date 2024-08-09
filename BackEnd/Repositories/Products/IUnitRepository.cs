using BackEnd.DTOs;
using BackEnd.Entities.Products;

namespace BackEnd.Repositories.Products
{
    public interface IUnitRepository
    {
        Task<List<Unit>> GetAll(PaginationDTO paginationDTO);
        Task<Unit?> GetById(int id);
        Task<int> Create(Unit unit);
        Task Update(Unit unit);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
    }
}
