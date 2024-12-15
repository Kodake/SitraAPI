using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
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
            group.MapGet("/item/{itemId:long}", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricing-items-get"))
                .AddPaginationParamsOpenAPI();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingItemDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPatch("/{id:long}/activate", Activate);            
            group.MapDelete("/{id:long}", Delete);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<PricingItemListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromRoute] long itemId, PaginationDTO pagination)
        {
            var pricingItems = await unitOfWork.PricingItemRepository.GetAllAsync(pagination, itemId);
            var pricingItemsDTO = mapper.Map<List<PricingItemListDTO>>(pricingItems);

            return TypedResults.Ok(pricingItemsDTO);
        }

        static async Task<Results<Created<PricingItemDTO>, ValidationProblem>> Create([FromBody] CreatePricingItemDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var pricingItem = mapper.Map<PricingItem>(dto);

            await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);

            var pricingItemDTO = mapper.Map<PricingItemDTO>(pricingItem);

            return TypedResults.Created($"/pricingItems/{pricingItem.Id}", pricingItemDTO);
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] long id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var success = await unitOfWork.PricingItemRepository.ActivateAsync(id);

            if (!success)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("products-get", default);
            await outputCacheStore.EvictByTagAsync("services-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Delete([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var pricingItemDB = await unitOfWork.PricingItemRepository.GetByIdAsync(id);

            if (pricingItemDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.PricingItemRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);

            return TypedResults.Ok();
        }
    }
}
