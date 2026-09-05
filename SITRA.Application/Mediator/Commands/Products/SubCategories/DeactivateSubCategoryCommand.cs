using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.SubCategories
{
    public class DeactivateSubCategoryCommand : ToggleCommand
    {
        public DeactivateSubCategoryCommand(Guid id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var subCategory = await unitOfWork.SubCategoryRepository.GetByIdAsync(Id);

            if (subCategory == null) return false;

            await unitOfWork.SubCategoryRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("subCategories-get", cancellationToken);

            return true;
        }
    }
}
