using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Finances.Taxes
{
    public class ActivateTaxCommand : ToggleCommand
    {
        public ActivateTaxCommand(int id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var tax = await unitOfWork.TaxRepository.GetByIdAsync(Id);

            if (tax == null) return false;

            await unitOfWork.TaxRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", cancellationToken);

            return true;
        }
    }
}
