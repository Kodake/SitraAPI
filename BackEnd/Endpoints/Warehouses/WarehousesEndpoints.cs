using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Filters;
using BackEnd.Helpers;
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

        static async Task<Ok<List<WarehouseListDTO>>> GetAll([FromServices] IWarehouseRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var warehouses = await repository.GetAll(pagination);
            var dto = mapper.Map<List<WarehouseListDTO>>(warehouses);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<WarehouseDTO>, NotFound>> GetById([FromServices] IWarehouseRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var warehouse = await repository.GetById(id);

            if (warehouse is null)
            {
                return TypedResults.NotFound();
            }

            var warehouseDTO = mapper.Map<WarehouseDTO>(warehouse);
            return TypedResults.Ok(warehouseDTO);
        }

        static async Task<Results<Created<WarehouseDTO>, ValidationProblem>> Create([FromBody] CreateWarehouseDTO dto,
           [FromServices] IWarehouseRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var warehouse = mapper.Map<Warehouse>(dto);

            var id = await repository.Create(warehouse);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);
            var warehouseDTO = mapper.Map<WarehouseDTO>(warehouse);
            return TypedResults.Created($"/warehouses/{id}", warehouseDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateWarehouseDTO dto,
            [FromServices] IWarehouseRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var warehouseDB = await repository.GetById(id);

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

            await repository.Update(warehouseDB);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IWarehouseRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var warehouseDB = await repository.GetById(id);

            if (warehouseDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IWarehouseRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var warehouseDB = await repository.GetById(id);

            if (warehouseDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);
            return TypedResults.Ok();
        }
    }
}
