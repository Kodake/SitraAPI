using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using BackEnd.MediatR.Commands.AdministrativeTools.Deadlines;
using BackEnd.MediatR.Queries.AdministrativeTools.Deadlines;
using MediatR;

namespace BackEnd.Endpoints
{
    public static class DeadlinesEndpoints
    {
        public static RouteGroupBuilder MapDeadlines(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("deadlines-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("deadlines-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<DeadlineListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllDeadlinesQuery(pagination);
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }

        static async Task<Ok<List<DeadlineListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsDeadlinesQuery(status);
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }

        static async Task<Results<Ok<DeadlineDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            int id)
        {
            var query = new GetDeadlineByIdQuery(id);
            var result = await mediator.Send(query);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateDeadlineDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateDeadlineCommand(dto);
            var deadlineDTO = await mediator.Send(command);
            return deadlineDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            int id, [FromBody] CreateDeadlineDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateDeadlineCommand(id, dto);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateDeadlineCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateDeadlineCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
