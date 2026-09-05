using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Models
{
    public class DeactivateModelCommand : ToggleCommand
    {
        public DeactivateModelCommand(Guid id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var model = await unitOfWork.ModelRepository.GetByIdAsync(Id);

            if (model == null) return false;

            await unitOfWork.ModelRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("models-get", cancellationToken);

            return true;
        }
    }
}
