using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class UnitsEndpoints
    {
        public static RouteGroupBuilder MapUnits(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("units-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("units-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateUnitDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateUnitDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<UnitListDTO>>> GetAll(IUnitRepository repository,
            IMapper mapper, PaginationDTO paginacion)
        {
            var units = await repository.GetAll(paginacion);
            var unitsDTO = mapper.Map<List<UnitListDTO>>(units);
            return TypedResults.Ok(unitsDTO);
        }

        static async Task<Ok<List<UnitListDTO>>> GetSelects(IUnitRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var units = await repository.GetAll(status);
            var unitsDTO = mapper.Map<List<UnitListDTO>>(units);
            return TypedResults.Ok(unitsDTO);
        }

        static async Task<Results<Ok<UnitDTO>, NotFound>> GetById(IUnitRepository repository,
            IMapper mapper, int id)
        {
            var unit = await repository.GetById(id);

            if (unit is null)
            {
                return TypedResults.NotFound();
            }

            var unitDTO = mapper.Map<UnitDTO>(unit);
            return TypedResults.Ok(unitDTO);
        }

        static async Task<Results<Created<UnitDTO>, ValidationProblem>> Create([FromForm] CreateUnitDTO dto,
           [FromServices] IUnitRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var unit = mapper.Map<Unit>(dto);

            var id = await repository.Create(unit);
            await outputCacheStore.EvictByTagAsync("units-get", default);
            var unitDTO = mapper.Map<UnitDTO>(unit);
            return TypedResults.Created($"/units/{id}", unitDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromForm] CreateUnitDTO dto,
            [FromServices] IUnitRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var unitDB = await repository.GetById(id);

            if (unitDB is null)
            {
                return TypedResults.NotFound();
            }

            unitDB.Name = dto.Name;
            unitDB.ShortName = dto.ShortName;
            unitDB.IsActive = unitDB.IsActive;

            await repository.Update(unitDB);
            await outputCacheStore.EvictByTagAsync("units-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             IUnitRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var unitDB = await repository.GetById(id);

            if (unitDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("units-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             IUnitRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var unitDB = await repository.GetById(id);

            if (unitDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("units-get", default);
            return TypedResults.Ok();
        }
    }
}
