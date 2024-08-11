using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface IUnitRepository
    {
        Task<List<Unit>> GetAll(PaginationDTO paginationDTO);
        Task<List<Unit>> GetAll(StatusType status);
        Task<Unit?> GetById(int id);
        Task<int> Create(Unit unit);
        Task Update(Unit unit);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, string name);
    }
}
