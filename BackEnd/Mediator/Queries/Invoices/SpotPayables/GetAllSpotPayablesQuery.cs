using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SpotPayables;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.SpotPayables
{
    public class GetAllSpotPayablesQuery : GetAllQuery<SpotPayableListDTO>
    {
        public GetAllSpotPayablesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SpotPayableListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var spotPayables = await unitOfWork.SpotPayableRepository.GetAllAsync(Pagination);
            return mapper.Map<List<SpotPayableListDTO>>(spotPayables);
        }
    }
}