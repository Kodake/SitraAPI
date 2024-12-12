using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Finances;
using BackEnd.Entities.Finances;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class CurrenciesEndpoints
    {
        public static RouteGroupBuilder MapCurrencies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("currencies-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/selects", GetCurrencies);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateCurrencyDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateCurrencyDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<CurrencyListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var currencies = await unitOfWork.CurrencyRepository.GetAllAsync(pagination);
            var currenciesDTO = mapper.Map<List<CurrencyListDTO>>(currencies);

            return TypedResults.Ok(currenciesDTO);
        }

        static async Task<Results<Ok<CurrencyDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(id);

            if (currency is null)
            {
                return TypedResults.NotFound();
            }

            var currencyDTO = mapper.Map<CurrencyDTO>(currency);

            return TypedResults.Ok(currencyDTO);
        }

        static async Task<Ok<List<CurrencyListDTO>>> GetCurrencies(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper)
        {
            var currencies = await unitOfWork.CurrencyRepository.GetCurrencies();
            var currenciesDTO = mapper.Map<List<CurrencyListDTO>>(currencies);

            return TypedResults.Ok(currenciesDTO);
        }

        static async Task<Results<Created<CurrencyDTO>, ValidationProblem>> Create([FromBody] CreateCurrencyDTO dto,
           [FromServices] IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var currency = mapper.Map<Currency>(dto);

            await unitOfWork.CurrencyRepository.CreateAsync(currency);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", default);

            var currencyDTO = mapper.Map<CurrencyDTO>(currency);

            return TypedResults.Created($"/currencies/{currency.Id}", currencyDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateCurrencyDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(id);

            if (currencyDB is null)
            {
                return TypedResults.NotFound();
            }

            currencyDB.Rate = dto.Rate;
            currencyDB.Code = dto.Code;
            currencyDB.Name = dto.Name;
            currencyDB.IsActive = currencyDB.IsActive;

            unitOfWork.CurrencyRepository.Update(currencyDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(id);

            if (currencyDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.CurrencyRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IOutputCacheStore outputCacheStore)
        {
            var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(id);

            if (currency is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.CurrencyRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", default);

            return TypedResults.Ok();
        }
    }
}
