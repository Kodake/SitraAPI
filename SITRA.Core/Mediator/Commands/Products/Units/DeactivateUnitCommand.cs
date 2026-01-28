using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Units
{
    public class DeactivateUnitCommand : ToggleCommand
    {
        public DeactivateUnitCommand(Guid id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(Id);

            if (unitDB == null) return false;

            await unitOfWork.UnitRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", cancellationToken);

            return true;
        }
    }
}
