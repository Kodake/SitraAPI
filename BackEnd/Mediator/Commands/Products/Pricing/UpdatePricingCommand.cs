using AutoMapper;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Pricings
{
    public class UpdatePricingCommand : UpdateCommand<Pricing, CreatePricingDTO>
    {
        public UpdatePricingCommand(long id, CreatePricingDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var pricingDB = await unitOfWork.PricingRepository.GetByIdAsync(Id);

            if (pricingDB == null) return false;

            mapper.Map(Dto, pricingDB);
            unitOfWork.PricingRepository.Update(pricingDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricings-get", default);

            return true;
        }
    }
}
