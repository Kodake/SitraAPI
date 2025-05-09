using AutoMapper;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Pricings
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
