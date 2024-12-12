using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Finances;
using BackEnd.Entities.Finances;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
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

        static async Task<Ok<List<TaxListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var taxes = await unitOfWork.TaxRepository.GetAll(status);
            var taxesDTO = mapper.Map<List<TaxListDTO>>(taxes);

            return TypedResults.Ok(taxesDTO);
        }

        static async Task<Ok<List<TaxListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var taxes = await unitOfWork.TaxRepository.GetAllAsync(pagination);
            var taxesDTO = mapper.Map<List<TaxListDTO>>(taxes);

            return TypedResults.Ok(taxesDTO);
        }

        static async Task<Results<Ok<TaxDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var tax = await unitOfWork.TaxRepository.GetByIdAsync(id);

            if (tax is null)
            {
                return TypedResults.NotFound();
            }

            var taxDTO = mapper.Map<TaxDTO>(tax);

            return TypedResults.Ok(taxDTO);
        }

        static async Task<Results<Created<TaxDTO>, ValidationProblem>> Create([FromBody] CreateTaxDTO dto,
           [FromServices] IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var tax = mapper.Map<Tax>(dto);

            await unitOfWork.TaxRepository.CreateAsync(tax);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", default);

            var taxDTO = mapper.Map<TaxDTO>(tax);

            return TypedResults.Created($"/taxes/{tax.Id}", taxDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateTaxDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var taxDB = await unitOfWork.TaxRepository.GetByIdAsync(id);

            if (taxDB is null)
            {
                return TypedResults.NotFound();
            }

            taxDB.Name = dto.Name;
            taxDB.Amount = dto.Amount;
            taxDB.IsActive = taxDB.IsActive;

            unitOfWork.TaxRepository.Update(taxDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var taxDB = await unitOfWork.TaxRepository.GetByIdAsync(id);

            if (taxDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.TaxRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var taxDB = await unitOfWork.TaxRepository.GetByIdAsync(id);

            if (taxDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.TaxRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", default);

            return TypedResults.Ok();
        }
    }
}
