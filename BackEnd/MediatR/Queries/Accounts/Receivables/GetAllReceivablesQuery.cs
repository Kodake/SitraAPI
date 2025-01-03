using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Accounts.Receivables;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Receivables
{
    public class GetAllReceivablesQuery : GetAllQuery<ReceivableListDTO>
    {
        public GetAllReceivablesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ReceivableListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var receivables = await unitOfWork.ReceivableRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ReceivableListDTO>>(receivables);
        }
    }
}