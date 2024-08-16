using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Finances;
using BackEnd.Entities.Finances;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Finances;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class TaxesEndpoints
    {
        public static RouteGroupBuilder MapTaxes(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("taxes-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("taxes-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateTaxDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateTaxDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<TaxListDTO>>> GetSelects(ITaxRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var taxes = await repository.GetAll(status);
            var taxesDTO = mapper.Map<List<TaxListDTO>>(taxes);
            return TypedResults.Ok(taxesDTO);
        }

        static async Task<Ok<List<TaxListDTO>>> GetAll([FromServices] ITaxRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var taxes = await repository.GetAll(pagination);
            var taxesDTO = mapper.Map<List<TaxListDTO>>(taxes);
            return TypedResults.Ok(taxesDTO);
        }

        static async Task<Results<Ok<TaxDTO>, NotFound>> GetById([FromServices] ITaxRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var tax = await repository.GetById(id);

            if (tax is null)
            {
                return TypedResults.NotFound();
            }

            var taxDTO = mapper.Map<TaxDTO>(tax);
            return TypedResults.Ok(taxDTO);
        }

        static async Task<Results<Created<TaxDTO>, ValidationProblem>> Create([FromForm] CreateTaxDTO dto,
           [FromServices] ITaxRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var tax = mapper.Map<Tax>(dto);

            var id = await repository.Create(tax);
            await outputCacheStore.EvictByTagAsync("taxes-get", default);
            var taxDTO = mapper.Map<TaxDTO>(tax);
            return TypedResults.Created($"/taxes/{id}", taxDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromForm] CreateTaxDTO dto,
            [FromServices] ITaxRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var taxDB = await repository.GetById(id);

            if (taxDB is null)
            {
                return TypedResults.NotFound();
            }

            taxDB.Name = dto.Name;
            taxDB.Amount = dto.Amount;
            taxDB.IsActive = taxDB.IsActive;

            await repository.Update(taxDB);
            await outputCacheStore.EvictByTagAsync("taxes-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] ITaxRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var taxDB = await repository.GetById(id);

            if (taxDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("taxes-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] ITaxRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var taxDB = await repository.GetById(id);

            if (taxDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("taxes-get", default);
            return TypedResults.Ok();
        }
    }
}
