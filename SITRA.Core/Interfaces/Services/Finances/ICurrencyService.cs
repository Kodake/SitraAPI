using SITRA.Core.Entities.Finances;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Finances
{
    public interface ICurrencyService
    {
        Task<List<Currency>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<List<Currency>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
