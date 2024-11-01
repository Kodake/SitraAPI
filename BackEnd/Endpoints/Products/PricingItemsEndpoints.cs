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
    public static class PricingItemsEndpoints
    {
        public static RouteGroupBuilder MapPricingItems(this RouteGroupBuilder group)
        {
            group.MapGet("/{itemId:int}", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricing-items-get"))
                .AddPaginationParamsOpenAPI();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingItemDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);            
            group.MapDelete("/{id:int}", Delete);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<PricingItemListDTO>>> GetAll(IPricingItemRepository repository,
            IMapper mapper, [FromRoute] int itemId, PaginationDTO paginacion)
        {
            var pricingItems = await repository.GetAll(paginacion, itemId);
            var pricingItemsDTO = mapper.Map<List<PricingItemListDTO>>(pricingItems);
            return TypedResults.Ok(pricingItemsDTO);
        }

        static async Task<Results<Created<PricingItemDTO>, ValidationProblem>> Create([FromBody] CreatePricingItemDTO dto,
           IPricingItemRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var pricingItem = mapper.Map<PricingItem>(dto);

            var id = await repository.Create(pricingItem);
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            var pricingItemDTO = mapper.Map<PricingItemDTO>(pricingItem);
            return TypedResults.Created($"/pricingItems/{id}", pricingItemDTO);
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             IPricingItemRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var success = await repository.Activate(id);

            if (!success)
            {
                return TypedResults.NotFound();
            }

            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("products-get", default);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Delete([FromRoute] int id,
             IPricingItemRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var pricingItemDB = await repository.GetById(id);

            if (pricingItemDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            return TypedResults.Ok();
        }
    }
}
