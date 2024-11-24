using BackEnd.DTOs;
using BackEnd.Entities.Finances;
using BackEnd.Enums;

namespace BackEnd.Repositories.Finances
{
    public interface ITaxRepository
    {
        Task<List<Tax>> GetAll(StatusType status);
        Task<Tax?> GetById(long id);
        Task<List<Tax>> GetAll(PaginationDTO paginationDTO);
        Task<long> Create(Tax category);
        Task Update(Tax category);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
    }
}
