using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.CashBoxes
{
    public interface IPettyCashMovementRepository : IRepositoryBase<PettyCashMovement>
    {
        Task<List<PettyCashMovement>> GetAllAsync(PaginationDTO paginationDTO, Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<List<PettyCashMovement>> GetReadOnlyAsync(PaginationDTO paginationDTO, Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<List<PettyCashMovement>> GetAllWithdrawalsAsync(PaginationDTO paginationDTO, Guid pettyCashId, CancellationToken cancellationToken = default);
        Task<double> GetTotalAmountPaidAsync(Guid pettyCashId, CancellationToken cancellationToken = default);
    }
}
