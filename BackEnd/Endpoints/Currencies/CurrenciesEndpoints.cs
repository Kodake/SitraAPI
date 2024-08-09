using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Currencies;
using BackEnd.Entities.Currencies;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Currencies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class CurrenciesEndpoints
    {
        public static RouteGroupBuilder MapCurrencies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("currencies-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:int}", GetById);
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

        static async Task<Ok<List<CurrencyListDTO>>> GetAll([FromServices] ICurrencyRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var currencies = await repository.GetAll(pagination);
            var currenciesDTO = mapper.Map<List<CurrencyListDTO>>(currencies);
            return TypedResults.Ok(currenciesDTO);
        }

        static async Task<Results<Ok<CurrencyDTO>, NotFound>> GetById([FromServices] ICurrencyRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var currency = await repository.GetById(id);

            if (currency is null)
            {
                return TypedResults.NotFound();
            }

            var currencyDTO = mapper.Map<CurrencyDTO>(currency);
            return TypedResults.Ok(currencyDTO);
        }

        static async Task<Results<Created<CurrencyDTO>, ValidationProblem>> Create([FromForm] CreateCurrencyDTO createCurrencyDTO,
           [FromServices] ICurrencyRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var currency = mapper.Map<Currency>(createCurrencyDTO);

            var id = await repository.Create(currency);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            var currencyDTO = mapper.Map<CurrencyDTO>(currency);
            return TypedResults.Created($"/currencies/{id}", currencyDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromForm] CreateCurrencyDTO createCurrencyDTO,
            [FromServices] ICurrencyRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var existingCurrency = await repository.GetById(id);

            if (existingCurrency is null)
            {
                return TypedResults.NotFound();
            }

            existingCurrency.Code = createCurrencyDTO.Code;
            existingCurrency.Name = createCurrencyDTO.Name;
            existingCurrency.IsDeleted = existingCurrency.IsDeleted;

            await repository.Update(existingCurrency);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] ICurrencyRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var currencyDB = await repository.GetById(id);

            if (currencyDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] ICurrencyRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var currencyDB = await repository.GetById(id);

            if (currencyDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("currencies-get", default);
            return TypedResults.Ok();
        }
    }
}
