using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.CashBoxes
{
    public class GetAllPettyCashMovementsByIdQuery(PaginationDTO pagination, long pettyCashId) : GetAllQuery<PettyCashMovementListDTO>(pagination)
    {
        public long PettyCashId { get; } = pettyCashId;

        public override async Task<List<PettyCashMovementListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pettyCashMovements = await unitOfWork.PettyCashMovementRepository.GetAllAsync(Pagination, PettyCashId);
            return mapper.Map<List<PettyCashMovementListDTO>>(pettyCashMovements);
        }
    }
}
