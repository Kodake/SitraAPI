using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Finances.Currencies
{
    public class ActivateCurrencyCommand : ToggleCommand
    {
        public ActivateCurrencyCommand(int id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(Id);

            if (currency == null) return false;

            await unitOfWork.CurrencyRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", cancellationToken);

            return true;
        }
    }
}
