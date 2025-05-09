using AutoMapper;
using BackEnd.DTOs.Invoices.SpotPayables;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Invoices.SpotPayables
{
    public class GetSpotPayableByIdQuery : GetByIdQuery<SpotPayableDTO>
    {
        public GetSpotPayableByIdQuery(long id) : base(id) { }

        public override async Task<SpotPayableDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var spotPayable = await unitOfWork.SpotPayableRepository.GetByIdAsync(Id);
            return mapper.Map<SpotPayableDTO>(spotPayable);
        }
    }
}
