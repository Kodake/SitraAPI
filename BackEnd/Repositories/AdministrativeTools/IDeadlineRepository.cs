using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Repositories.AdministrativeTools
{
    public interface IDeadlineRepository
    {
        Task<List<Deadline>> GetAll(PaginationDTO paginationDTO);
        Task<List<Deadline>> GetAll(StatusType status);
        Task<Deadline?> GetById(int id);
        Task<int> Create(Deadline deadline);
        Task Update(Deadline deadline);
        Task Activate(int id);
        Task Deactivate(int id);
        Task<bool> Exists(int id);
        Task<bool> Exists(int id, int days);
    }
}
