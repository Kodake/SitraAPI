using SITRA.Core.Entities.Accounts;

namespace SITRA.Core.Interfaces.Services.Accounts
{
    public interface IReceivableService
    {
        Task<Receivable?> GetPaysByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Receivable>> GetPaginatedAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
