using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports
{
    public class ActivateFiscalSupportCommand : ToggleCommand
    {
        public ActivateFiscalSupportCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(Id);

            if (fiscalSupportDB == null) return false;

            await unitOfWork.FiscalSupportRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscalsupports-get", cancellationToken);

            return true;
        }
    }
}
