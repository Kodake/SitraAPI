using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.CashBoxes
{
    public class GetAllWithdrawalPettyCashMovementsByIdQuery(PaginationDTO pagination, Guid pettyCashId) : GetAllQuery<WithdrawalPettyCashMovementListDTO>(pagination)
    {
        public Guid PettyCashId { get; } = pettyCashId;

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
