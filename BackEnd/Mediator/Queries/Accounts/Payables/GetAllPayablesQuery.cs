using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Accounts.Payables
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
