using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.Mediator.Commands.Products.PricingItems;
using SITRA.Core.Mediator.Queries.Products.PricingItems;

namespace SITRA.API.Endpoints
{
    public static class PricingItemsEndpoints
    {
        public static RouteGroupBuilder MapPricingItems(this RouteGroupBuilder group)
        {
            group.MapGet("/item/{itemId:guid}", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricing-items-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingItemDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}", Delete).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<PricingItemListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            [FromRoute] Guid itemId,
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
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivatePricingItemCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Delete(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeletePricingItemCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

