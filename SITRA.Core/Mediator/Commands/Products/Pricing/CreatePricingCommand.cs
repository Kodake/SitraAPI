using AutoMapper;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Pricings
{
    public class CreatePricingCommand : CreateCommand<Pricing, CreatePricingDTO>
    {
        public CreatePricingCommand(CreatePricingDTO dto) : base(dto) { }

        public override async Task<CreatePricingDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var pricing = mapper.Map<Pricing>(Dto);
            await unitOfWork.PricingRepository.CreateAsync(pricing);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);
            return Dto;
        }
    }
}
