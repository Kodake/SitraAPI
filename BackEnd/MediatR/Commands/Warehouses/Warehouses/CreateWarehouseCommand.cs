using AutoMapper;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Warehouses.Warehouses
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
