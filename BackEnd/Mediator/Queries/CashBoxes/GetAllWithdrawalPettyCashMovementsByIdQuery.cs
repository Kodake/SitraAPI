using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.CashBoxes
{
    public class GetAllWithdrawalPettyCashMovementsByIdQuery(PaginationDTO pagination, long pettyCashId) : GetAllQuery<WithdrawalPettyCashMovementListDTO>(pagination)
    {
        public long PettyCashId { get; } = pettyCashId;

        public override async Task<List<WithdrawalPettyCashMovementListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pettyCashMovements = await unitOfWork.PettyCashMovementRepository.GetAllWithdrawalsAsync(Pagination, PettyCashId);
            return mapper.Map<List<WithdrawalPettyCashMovementListDTO>>(pettyCashMovements);
        }
    }
}