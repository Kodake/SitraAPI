using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Models
{
    public class ActivateModelCommand : ToggleCommand
    {
        public ActivateModelCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var model = await unitOfWork.ModelRepository.GetByIdAsync(Id);

            if (model == null) return false;

            await unitOfWork.ModelRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("models-get", cancellationToken);

            return true;
        }
    }
}
