using SITRA.Core.Entities.Accounts;

namespace SITRA.Core.Interfaces.Services.Accounts
{
    public interface IPayableService
    {
        Task<Payable?> GetPaysByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Payable>> GetPaginatedAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
