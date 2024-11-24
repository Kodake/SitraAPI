using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using System.Runtime.CompilerServices;

namespace BackEnd.Repositories.AdministrativeTools
{
    public interface IDeadlineRepository
    {
        Task<List<Deadline>> GetAll(PaginationDTO paginationDTO);
        Task<List<Deadline>> GetAll(StatusType status);
        Task<Deadline?> GetById(long id);
        Task<long> Create(Deadline deadline);
        Task Update(Deadline deadline);
        Task Activate(long id);
        Task Deactivate(long id);
        Task<bool> Exists(long id);
        Task<bool> Exists(long id, long days);
    }
}
