using AutoMapper;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Pricings
{
    public class GetUnregisteredPricingsForItemQuery : GetAllByIdQuery<PricingListDTO>
    {
        public GetUnregisteredPricingsForItemQuery(Guid id) : base(id) { }

        public override async Task<IEnumerable<PricingListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pricings = await unitOfWork.PricingRepository.GetUnregisteredPricingsForItem(Id);
            return mapper.Map<IEnumerable<PricingListDTO>>(pricings);
        }
    }
}
