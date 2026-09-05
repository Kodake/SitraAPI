using AutoMapper;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Pricings
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
