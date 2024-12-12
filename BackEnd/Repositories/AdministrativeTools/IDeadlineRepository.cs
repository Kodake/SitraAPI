using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.AdministrativeTools
{
    public interface IDeadlineRepository : IRepositoryBase<Deadline>
    {
        Task<List<Deadline>> GetAll(StatusType status);
        Task<bool> Exists(long id, long days);
    }
}
