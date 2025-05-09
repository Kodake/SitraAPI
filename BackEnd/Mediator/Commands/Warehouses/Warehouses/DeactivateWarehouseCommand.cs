using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Warehouses.Warehouses
{
    public class DeactivateWarehouseCommand : ToggleCommand
    {
        public DeactivateWarehouseCommand(long id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var warehouseDB = await unitOfWork.WarehouseRepository.GetByIdAsync(Id);

            if (warehouseDB == null) return false;

            await unitOfWork.WarehouseRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("warehouses-get", cancellationToken);

            return true;
        }
    }
}
