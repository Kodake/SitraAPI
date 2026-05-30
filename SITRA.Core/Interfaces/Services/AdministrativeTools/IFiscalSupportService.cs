using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.AdministrativeTools
{
    public interface IFiscalSupportService
    {
        Task<List<FiscalSupport>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
    }
}
