using SITRA.Core.Entities.Finances;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Finances
{
    public interface ITaxService
    {
        Task<List<Tax>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
    }
}
