using BackEnd.DTOs;
using BackEnd.Entities.Finances;
using BackEnd.Enums;

namespace BackEnd.Repositories.Finances
{
    public interface ITaxRepository
    {
        Task<List<Tax>> GetAll(StatusType status);
        Task<Tax?> GetById(int id);
        Task<List<Tax>> GetAll(PaginationDTO paginationDTO);
        Task<int> Create(Tax category);
        Task Update(Tax category);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
    }
}
