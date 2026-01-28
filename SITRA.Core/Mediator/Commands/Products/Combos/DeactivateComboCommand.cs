using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Combos
{
    public class DeactivateComboCommand : ToggleCommand
    {
        public DeactivateComboCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var comboDB = await unitOfWork.ComboRepository.GetByIdAsync(Id);

            if (comboDB == null) return false;

            await unitOfWork.ComboRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("combos-get", cancellationToken);

            return true;
        }
    }
}
