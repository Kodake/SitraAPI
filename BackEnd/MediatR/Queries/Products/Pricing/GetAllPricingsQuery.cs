using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Pricings
{
    public class GetAllPricingsQuery : GetAllQuery<PricingListDTO>
    {
        public GetAllPricingsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<PricingListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pricings = await unitOfWork.PricingRepository.GetAllAsync(Pagination);
            return mapper.Map<List<PricingListDTO>>(pricings);
        }
    }
}
