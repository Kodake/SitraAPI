using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SpotPayables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SpotPayables
{
    public class GetAllSpotPayablesQuery : GetAllQuery<SpotPayableListDTO>
    {
        public GetAllSpotPayablesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SpotPayableListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var spotPayables = await unitOfWork.SpotPayableRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<SpotPayableListDTO>>(spotPayables);
        }
    }
}
