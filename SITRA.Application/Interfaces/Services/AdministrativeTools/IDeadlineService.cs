using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.AdministrativeTools
{
    public interface IDeadlineService
    {
        Task<List<Deadline>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
    }
}
