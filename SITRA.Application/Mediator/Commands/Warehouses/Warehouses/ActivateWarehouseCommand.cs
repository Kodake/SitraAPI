using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Warehouses.Warehouses
{
    public class ActivateWarehouseCommand : ToggleCommand
    {
        public ActivateWarehouseCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var warehouseDB = await unitOfWork.WarehouseRepository.GetByIdAsync(Id);

            if (warehouseDB == null) return false;

            await unitOfWork.WarehouseRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("warehouses-get", cancellationToken);

            return true;
        }
    }
}
