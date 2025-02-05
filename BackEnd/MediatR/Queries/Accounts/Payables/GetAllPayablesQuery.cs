using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Repositories;
using BackEnd.DTOs.Accounts.Payables;

namespace BackEnd.MediatR.Queries.Accounts.Payables
{
    public class GetAllPayablesQuery : GetAllQuery<PayableListDTO>
    {
        public GetAllPayablesQuery(PaginationDTO pagination) : base(pagination) { }

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
