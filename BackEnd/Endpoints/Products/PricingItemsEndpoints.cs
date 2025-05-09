using BackEnd.DTOs;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Products.PricingItems;
using BackEnd.Mediator.Queries.Products.PricingItems;
using MediatR;
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
                .WithOpenApi();
            group.MapPatch("/{id:long}/activate", Activate);
            group.MapDelete("/{id:long}", Delete);
            return group;
        }

        static async Task<Ok<List<PricingItemListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            [FromRoute] long itemId,
            PaginationDTO pagination)
        {
            var query = new GetAllPricingItemsByIdQuery(pagination, itemId);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePricingItemDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreatePricingItemCommand(dto);
            var pricingItemDTO = await mediator.Send(command);
            return pricingItemDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivatePricingItemCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Delete(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeletePricingItemCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
