using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Warehouses.Warehouses
{
    public class CreateWarehouseCommand : CreateCommand<Warehouse, CreateWarehouseDTO>
    {
        public CreateWarehouseCommand(CreateWarehouseDTO dto) : base(dto) { }

        public override async Task<CreateWarehouseDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var warehouse = mapper.Map<Warehouse>(Dto);
            await unitOfWork.WarehouseRepository.CreateAsync(warehouse);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("warehouses-get", cancellationToken);
            return Dto;
        }
    }
}
