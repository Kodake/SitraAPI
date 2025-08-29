using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Combos
{
    public class DeactivateComboCommand : ToggleCommand
    {
        public DeactivateComboCommand(long id) : base(id) { }

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
