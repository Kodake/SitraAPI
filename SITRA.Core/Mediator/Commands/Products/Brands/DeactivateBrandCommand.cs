using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Brands
{
    public class DeactivateBrandCommand : ToggleCommand
    {
        public DeactivateBrandCommand(long id) : base(id)
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
