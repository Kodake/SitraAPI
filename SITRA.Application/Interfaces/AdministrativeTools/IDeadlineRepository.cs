using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IDeadlineRepository : IRepositoryBase<Deadline>
    {
        Task<bool> Exists(Guid id, long days, CancellationToken cancellationToken = default);
    }
}

