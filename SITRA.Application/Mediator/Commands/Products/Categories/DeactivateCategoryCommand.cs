using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Categories
{
    public class DeactivateCategoryCommand : ToggleCommand
    {
        public DeactivateCategoryCommand(Guid id) : base(id)
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
