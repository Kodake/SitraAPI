using AutoMapper;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Pricings
{
    public class GetUnregisteredPricingsForItemQuery : GetAllByIdQuery<PricingListDTO>
    {
        public GetUnregisteredPricingsForItemQuery(long id) : base(id) { }

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
