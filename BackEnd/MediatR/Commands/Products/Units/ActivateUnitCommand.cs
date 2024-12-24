using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.Units
{
    public class ActivateUnitCommand : ToggleCommand
    {
        public ActivateUnitCommand(int id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var unit = await unitOfWork.UnitRepository.GetByIdAsync(Id);

            if (unit == null) return false;

            await unitOfWork.UnitRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", cancellationToken);

            return true;
        }
    }
}
