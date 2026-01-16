using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.PricingItems
{
    public class GetAllPricingItemsByIdQuery(PaginationDTO pagination, long itemId) : GetAllQuery<PricingItemListDTO>(pagination)
    {
        public long ItemId { get; } = itemId;

        public override async Task<List<PricingItemListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var pricingItems = await unitOfWork.PricingItemRepository.GetAllAsync(Pagination, ItemId);
            return mapper.Map<List<PricingItemListDTO>>(pricingItems);
        }
    }
}
