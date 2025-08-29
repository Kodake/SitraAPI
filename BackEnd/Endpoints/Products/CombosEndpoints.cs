using BackEnd.DTOs;
using BackEnd.DTOs.Products.Combos;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Products.Combos;
using BackEnd.Mediator.Queries.Products.Combos;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class CombosEndpoints
    {
        public static RouteGroupBuilder MapCombos(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("combos-get"))
                .AddPaginationParamsOpenAPI();
            //.RequireAuthorization();
            group.MapGet("/{id:long}", GetById);
            //.RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateComboDTO>>()
                .WithOpenApi();
                //.RequireAuthorization();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateComboDTO>>()
                .WithOpenApi()
                .RequireAuthorization();
            group.MapPatch("/{id:long}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:long}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ComboListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllCombosQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ComboDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetComboByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateComboDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateComboCommand(dto);
            var productDTO = await mediator.Send(command);
            return productDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateComboDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateComboCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateComboCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateComboCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}