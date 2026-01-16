using AutoMapper;
using SITRA.Core.DTOs.Invoices.SpotPayables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SpotPayables
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
