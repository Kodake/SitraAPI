using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.SubCategories
{
    public class ActivateSubCategoryCommand : ToggleCommand
    {
        public ActivateSubCategoryCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var subCategory = await unitOfWork.SubCategoryRepository.GetByIdAsync(Id);

            if (subCategory == null) return false;

            await unitOfWork.SubCategoryRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("subCategories-get", cancellationToken);

            return true;
        }
    }
}
