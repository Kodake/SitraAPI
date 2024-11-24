using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.Products
{
    public interface IUnitRepository
    {
        Task<List<Unit>> GetAll(PaginationDTO paginationDTO);
        Task<List<Unit>> GetAll(StatusType status);
        Task<Unit?> GetById(long id);
        Task<long> Create(Unit unit);
        Task Update(Unit unit);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, string name);
    }
}
