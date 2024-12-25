using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.PricingItems
{
    public class ActivatePricingItemCommand : ToggleCommand
    {
        public ActivatePricingItemCommand(long id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var pricingItemDB = await unitOfWork.PricingItemRepository.GetByIdAsync(Id);

            if (pricingItemDB == null) return false;

            await unitOfWork.PricingItemRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("products-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("services-get", cancellationToken);

            return true;
        }
    }
}
