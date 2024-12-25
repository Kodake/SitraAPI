using BackEnd.DTOs;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.Products.Pricings;
using BackEnd.MediatR.Queries.Products.Pricings;
using MediatR;
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
            group.MapGet("/unregistered-pricings/{id:long}", GetUnregisteredPricingsForItem).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricings-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapGet("/selects", GetPricings);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                .WithOpenApi();
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
            long id)
        {
            var query = new GetUnregisteredPricingsForItemQuery(id);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<PricingDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
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
            long id,
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
