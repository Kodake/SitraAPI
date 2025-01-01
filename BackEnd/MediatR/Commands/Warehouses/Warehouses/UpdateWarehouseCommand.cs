using AutoMapper;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Warehouses.Warehouses
{
    public class UpdateWarehouseCommand : UpdateCommand<Warehouse, CreateWarehouseDTO>
    {
        public UpdateWarehouseCommand(long id, CreateWarehouseDTO dto) : base(id, dto) { }

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
