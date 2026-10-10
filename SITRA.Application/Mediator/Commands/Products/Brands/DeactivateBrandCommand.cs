using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Brands
{
    public class DeactivateBrandCommand : ToggleCommand
    {
        public DeactivateBrandCommand(Guid id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var brand = await unitOfWork.BrandRepository.GetByIdAsync(Id);

            if (brand == null) return false;

            await unitOfWork.BrandRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", cancellationToken);

            return true;
        }
    }
}
