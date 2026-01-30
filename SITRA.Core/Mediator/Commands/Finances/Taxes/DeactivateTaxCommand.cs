using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Finances.Taxes
{
    public class DeactivateTaxCommand : ToggleCommand
    {
        public DeactivateTaxCommand(Guid id) : base(id)
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
