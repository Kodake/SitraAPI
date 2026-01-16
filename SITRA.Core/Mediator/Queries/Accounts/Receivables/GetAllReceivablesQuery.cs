using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Receivables
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
