using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools;
using BackEnd.DTOs.Products;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class FiscalSupportsEndpoints
    {
        public static RouteGroupBuilder MapFiscalSupports(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscal-supports-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscal-supports-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<FiscalSupportListDTO>>> GetAll([FromServices] IFiscalSupportRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var fiscalSupports = await repository.GetAll(pagination);
            var fiscalSupportsDTO = mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);
            return TypedResults.Ok(fiscalSupportsDTO);
        }

        static async Task<Ok<List<FiscalSupportListDTO>>> GetSelects(IFiscalSupportRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var fiscalSupports = await repository.GetAll(status);
            var fiscalSupportsDTO = mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);
            return TypedResults.Ok(fiscalSupportsDTO);
        }

        static async Task<Results<Ok<FiscalSupportDTO>, NotFound>> GetById([FromServices] IFiscalSupportRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var fiscalSupport = await repository.GetById(id);

            if (fiscalSupport is null)
            {
                return TypedResults.NotFound();
            }

            var fiscalSupportDTO = mapper.Map<FiscalSupportDTO>(fiscalSupport);
            return TypedResults.Ok(fiscalSupportDTO);
        }

        static async Task<Results<Created<FiscalSupportDTO>, ValidationProblem>> Create([FromBody] CreateFiscalSupportDTO dto,
           [FromServices] IFiscalSupportRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var fiscalSupport = mapper.Map<FiscalSupport>(dto);

            var id = await repository.Create(fiscalSupport);
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);
            var fiscalSupportDTO = mapper.Map<FiscalSupportDTO>(fiscalSupport);
            return TypedResults.Created($"/fiscalSupports/{id}", fiscalSupportDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateFiscalSupportDTO dto,
            [FromServices] IFiscalSupportRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var fiscalSupportDB = await repository.GetById(id);

            if (fiscalSupportDB is null)
            {
                return TypedResults.NotFound();
            }

            fiscalSupportDB.DocumentType = dto.DocumentType;
            fiscalSupportDB.FiscalSupportType = dto.FiscalSupportType;
            fiscalSupportDB.Name = dto.Name;
            fiscalSupportDB.SpaceNumber = dto.SpaceNumber;
            fiscalSupportDB.Symbol = dto.Symbol;
            fiscalSupportDB.WarehouseId = dto.WarehouseId;
            fiscalSupportDB.InitialSequence = dto.InitialSequence;
            fiscalSupportDB.FinalSequence = dto.FinalSequence;
            fiscalSupportDB.IsActive = fiscalSupportDB.IsActive;

            await repository.Update(fiscalSupportDB);
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IFiscalSupportRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var fiscalSupportDB = await repository.GetById(id);

            if (fiscalSupportDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IFiscalSupportRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var fiscalSupportDB = await repository.GetById(id);

            if (fiscalSupportDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);
            return TypedResults.Ok();
        }
    }
}
