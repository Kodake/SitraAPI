using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class PricingsEndpoints
    {
        public static RouteGroupBuilder MapPricings(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricings-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<PricingListDTO>>> GetAll(IPricingRepository repository,
            IMapper mapper, PaginationDTO paginacion)
        {
            var pricings = await repository.GetAll(paginacion);
            var pricingsDTO = mapper.Map<List<PricingListDTO>>(pricings);
            return TypedResults.Ok(pricingsDTO);
        }

        static async Task<Results<Ok<PricingDTO>, NotFound>> GetById(IPricingRepository repository,
            IMapper mapper, int id)
        {
            var pricing = await repository.GetById(id);

            if (pricing is null)
            {
                return TypedResults.NotFound();
            }

            var pricingDTO = mapper.Map<PricingDTO>(pricing);
            return TypedResults.Ok(pricingDTO);
        }

        static async Task<Results<Created<PricingDTO>, ValidationProblem>> Create([FromBody] CreatePricingDTO dto,
           IPricingRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var pricing = mapper.Map<Pricing>(dto);

            var pricingSaved = await repository.Create(pricing);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            var pricingDTO = mapper.Map<PricingDTO>(pricing);
            return TypedResults.Created($"/pricings/{pricingSaved.Id}", pricingDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreatePricingDTO dto,
           IPricingRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var pricingDB = await repository.GetById(id);

            if (pricingDB is null)
            {
                return TypedResults.NotFound();
            }

            pricingDB.Name = dto.Name;
            pricingDB.PricingType = dto.PricingType;
            pricingDB.Percentage = dto.Percentage;

            await repository.Update(pricingDB);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            return TypedResults.Ok();
        }
    }
}
