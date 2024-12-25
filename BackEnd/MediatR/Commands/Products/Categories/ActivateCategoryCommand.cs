using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.Categories
{
    public class ActivateCategoryCommand : ToggleCommand
    {
        public ActivateCategoryCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(Id);

            if (category == null) return false;

            await unitOfWork.CategoryRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", cancellationToken);

            return true;
        }
    }
}
