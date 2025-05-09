using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Finances.Taxes
{
    public class DeactivateTaxCommand : ToggleCommand
    {
        public DeactivateTaxCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var tax = await unitOfWork.TaxRepository.GetByIdAsync(Id);

            if (tax == null) return false;

            await unitOfWork.TaxRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", cancellationToken);

            return true;
        }
    }
}
