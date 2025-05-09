using AutoMapper;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Pricings
{
    public class GetSelectsPricingsQuery : GetSelectsQuery<PricingListDTO>
    {
        public GetSelectsPricingsQuery(StatusType status = StatusType.All) : base(status) { }

        public override async Task<List<PricingListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pricings = await unitOfWork.PricingRepository.GetAll();
            return mapper.Map<List<PricingListDTO>>(pricings);
        }
    }
}
