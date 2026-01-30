using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Combos
{
    public class ActivateComboCommand : ToggleCommand
    {
        public ActivateComboCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var comboDB = await unitOfWork.ComboRepository.GetByIdAsync(Id);

            if (comboDB == null) return false;

            await unitOfWork.ComboRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("combos-get", cancellationToken);

            return true;
        }
    }
}
