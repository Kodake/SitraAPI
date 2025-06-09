using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.SubCategories
{
    public class DeactivateSubCategoryCommand : ToggleCommand
    {
        public DeactivateSubCategoryCommand(long id) : base(id)
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
