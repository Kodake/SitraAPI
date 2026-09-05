using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Warehouses.Warehouses
{
    public class UpdateWarehouseCommand : UpdateCommand<Warehouse, CreateWarehouseDTO>
    {
        public UpdateWarehouseCommand(Guid id, CreateWarehouseDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var warehouseDB = await unitOfWork.WarehouseRepository.GetByIdAsync(Id);

            if (warehouseDB == null) return false;

            mapper.Map(Dto, warehouseDB);
            unitOfWork.WarehouseRepository.Update(warehouseDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("warehouses-get", cancellationToken);

            return true;
        }
    }
}
