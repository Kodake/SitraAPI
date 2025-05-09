using BackEnd.DTOs;
using BackEnd.Entities.CashBoxes;
using BackEnd.Repositories.Generics;

namespace BackEnd.Repositories.CashBoxes
{
    public interface IPettyCashMovementRepository : IRepositoryBase<PettyCashMovement>
    {
        Task<List<PettyCashMovement>> GetAllAsync(PaginationDTO paginationDTO, long pettyCashId);
        Task<double> GetTotalAmountPaidAsync(long pettyCashId);
    }
}
