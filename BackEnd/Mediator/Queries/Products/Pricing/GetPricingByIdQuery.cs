using AutoMapper;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Pricings
{
    public class GetPricingByIdQuery : GetByIdQuery<PricingDTO>
    {
        public GetPricingByIdQuery(long id) : base(id) { }

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
