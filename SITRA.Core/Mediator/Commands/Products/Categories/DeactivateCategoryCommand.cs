using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Categories
{
    public class DeactivateCategoryCommand : ToggleCommand
    {
        public DeactivateCategoryCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(Id);

            if (category == null) return false;

            await unitOfWork.CategoryRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", cancellationToken);

            return true;
        }
    }
}
