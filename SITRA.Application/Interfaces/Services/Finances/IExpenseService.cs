using SITRA.Core.Entities.Finances;
using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces.Services.Finances
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default);
        Task<List<Expense>> GetAllAsync(SITRA.Core.DTOs.PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
