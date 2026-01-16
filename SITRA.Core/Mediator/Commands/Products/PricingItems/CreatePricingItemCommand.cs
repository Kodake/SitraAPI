using AutoMapper;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.PricingItems
{
    public class CreatePricingItemCommand : CreateCommand<PricingItem, CreatePricingItemDTO>
    {
        public CreatePricingItemCommand(CreatePricingItemDTO dto) : base(dto) { }

        public override async Task<CreatePricingItemDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var pricingItem = mapper.Map<PricingItem>(Dto);
            await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("products-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("services-get", cancellationToken);
            return Dto;
        }
    }
}
