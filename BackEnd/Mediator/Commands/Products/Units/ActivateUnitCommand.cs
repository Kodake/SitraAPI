using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Units
{
    public class ActivateUnitCommand : ToggleCommand
    {
        public ActivateUnitCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(Id);

            if (unitDB == null) return false;

            await unitOfWork.UnitRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", cancellationToken);

            return true;
        }
    }
}
