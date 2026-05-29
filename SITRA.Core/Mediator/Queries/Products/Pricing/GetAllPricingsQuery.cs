using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Pricings
{
    public class GetAllPricingsQuery : GetAllQuery<PricingListDTO>
    {
        public GetAllPricingsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<PricingListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pricings = await unitOfWork.PricingRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<PricingListDTO>>(pricings);
        }
    }
}
