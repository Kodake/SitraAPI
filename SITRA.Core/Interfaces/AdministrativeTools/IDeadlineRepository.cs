using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IDeadlineRepository : IRepositoryBase<Deadline>
    {
        Task<List<Deadline>> GetAll(StatusType status);
        Task<bool> Exists(long id, long days);
    }
}
