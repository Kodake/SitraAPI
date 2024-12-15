using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.Products;
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

        static async Task<Ok<List<UnitListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var units = await unitOfWork.UnitRepository.GetAllAsync(pagination);
            var unitsDTO = mapper.Map<List<UnitListDTO>>(units);

            return TypedResults.Ok(unitsDTO);
        }

        static async Task<Ok<List<UnitListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var units = await unitOfWork.UnitRepository.GetAll(status);
            var unitsDTO = mapper.Map<List<UnitListDTO>>(units);

            return TypedResults.Ok(unitsDTO);
        }

        static async Task<Results<Ok<UnitDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var unit = await unitOfWork.UnitRepository.GetByIdAsync(id);

            if (unit is null)
            {
                return TypedResults.NotFound();
            }

            var unitDTO = mapper.Map<UnitDTO>(unit);

            return TypedResults.Ok(unitDTO);
        }

        static async Task<Results<Created<UnitDTO>, ValidationProblem>> Create([FromBody] CreateUnitDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var unit = mapper.Map<Unit>(dto);

            await unitOfWork.UnitRepository.CreateAsync(unit);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", default);

            var unitDTO = mapper.Map<UnitDTO>(unit);

            return TypedResults.Created($"/units/{unit.Id}", unitDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateUnitDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(id);

            if (unitDB is null)
            {
                return TypedResults.NotFound();
            }

            unitDB.Name = dto.Name;
            unitDB.ShortName = dto.ShortName;
            unitDB.IsActive = unitDB.IsActive;

            unitOfWork.UnitRepository.Update(unitDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(id);

            if (unitDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.UnitRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(id);

            if (unitDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.UnitRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", default);

            return TypedResults.Ok();
        }
    }
}
