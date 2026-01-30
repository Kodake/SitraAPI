using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Mediator.Commands.Products.Pricings;
using SITRA.Core.Mediator.Queries.Products.Pricings;

namespace SITRA.API.Endpoints
{
    public static class PricingsEndpoints
    {
        public static RouteGroupBuilder MapPricings(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricings-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/unregistered-pricings/{id:guid}", GetUnregisteredPricingsForItem).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricings-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapGet("/selects", GetPricings).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                .WithOpenApi().RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<PricingListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllPricingsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<IEnumerable<PricingListDTO>>> GetUnregisteredPricingsForItem(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetUnregisteredPricingsForItemQuery(id);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<PricingDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetPricingByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Ok<List<PricingListDTO>>> GetPricings(
            [FromServices] IMediator mediator)
        {
            var query = new GetSelectsPricingsQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePricingDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreatePricingCommand(dto);
            var pricingDTO = await mediator.Send(command);
            return pricingDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            Guid id,
            [FromBody] CreatePricingDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdatePricingCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

