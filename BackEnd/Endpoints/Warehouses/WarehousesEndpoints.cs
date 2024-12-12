using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.Warehouses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class WarehousesEndpoints
    {
        public static RouteGroupBuilder MapWarehouses(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/stock-warehouses/{id:int}", GetWarehouseStockByProductId);
            group.MapGet("/unregistered-warehouses/{id:int}", GetUnregisteredWarehouseById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get"));
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWarehouseDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWarehouseDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var warehouses = await unitOfWork.WarehouseRepository.GetAllAsync(pagination);
            var dto = mapper.Map<List<WarehouseListDTO>>(warehouses);

            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetUnregisteredWarehouseById(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var unregWarehouses = await unitOfWork.WarehouseRepository.GetUnregisteredWarehouseById(id);
            var unregWarehousesDTO = mapper.Map<List<WarehouseListDTO>>(unregWarehouses);

            return TypedResults.Ok(unregWarehousesDTO);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetWarehouseStockByProductId(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var stockWarehouses = await unitOfWork.WarehouseRepository.GetStockWarehouseByProductId(id);
            var stockWarehousesDTO = mapper.Map<List<WarehouseListDTO>>(stockWarehouses);

            return TypedResults.Ok(stockWarehousesDTO);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var warehouses = await unitOfWork.WarehouseRepository.GetAll(status);
            var warehousesDTO = mapper.Map<List<WarehouseListDTO>>(warehouses);

            return TypedResults.Ok(warehousesDTO);
        }

        static async Task<Results<Ok<WarehouseDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var warehouse = await unitOfWork.WarehouseRepository.GetByIdAsync(id);

            if (warehouse is null)
            {
                return TypedResults.NotFound();
            }

            var warehouseDTO = mapper.Map<WarehouseDTO>(warehouse);

            return TypedResults.Ok(warehouseDTO);
        }

        static async Task<Results<Created<WarehouseDTO>, ValidationProblem>> Create([FromBody] CreateWarehouseDTO dto,
           [FromServices] IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var warehouse = mapper.Map<Warehouse>(dto);

            await unitOfWork.WarehouseRepository.CreateAsync(warehouse);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);

            var warehouseDTO = mapper.Map<WarehouseDTO>(warehouse);

            return TypedResults.Created($"/warehouses/{warehouse.Id}", warehouseDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateWarehouseDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var warehouseDB = await unitOfWork.WarehouseRepository.GetByIdAsync(id);

            if (warehouseDB is null)
            {
                return TypedResults.NotFound();
            }

            warehouseDB.Name = dto.Name;
            warehouseDB.Address = dto.Address;
            warehouseDB.Phone = dto.Phone;
            warehouseDB.Email = dto.Email;
            warehouseDB.City = dto.City;
            warehouseDB.Country = dto.Country;
            warehouseDB.ZipCode = dto.ZipCode;
            warehouseDB.IsActive = warehouseDB.IsActive;

            unitOfWork.WarehouseRepository.Update(warehouseDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var warehouseDB = await unitOfWork.WarehouseRepository.GetByIdAsync(id);

            if (warehouseDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.WarehouseRepository.ActivateAsync(id);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var warehouseDB = await unitOfWork.WarehouseRepository.GetByIdAsync(id);

            if (warehouseDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.WarehouseRepository.DeactivateAsync(id);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);

            return TypedResults.Ok();
        }
    }
}
