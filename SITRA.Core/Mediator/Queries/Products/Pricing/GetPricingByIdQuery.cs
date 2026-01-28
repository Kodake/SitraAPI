using AutoMapper;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Pricings
{
    public class GetPricingByIdQuery : GetByIdQuery<PricingDTO>
    {
        public GetPricingByIdQuery(Guid id) : base(id) { }

        public override async Task<PricingDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pricing = await unitOfWork.PricingRepository.GetByIdAsync(Id);
            return mapper.Map<PricingDTO>(pricing);
        }
    }
}
