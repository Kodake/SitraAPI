using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Accounts.Payables
{
    public class GetAllPayablesQuery(PaginationDTO pagination) : GetAllQuery<PayableListDTO>(pagination)
    {
        public override async Task<List<PayableListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var payables = await unitOfWork.PayableRepository.GetAllAsync(Pagination);
            return mapper.Map<List<PayableListDTO>>(payables);
        }
    }
}
