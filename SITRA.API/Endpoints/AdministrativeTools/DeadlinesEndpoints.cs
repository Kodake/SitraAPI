using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Deadlines;
using SITRA.Core.Mediator.Queries.AdministrativeTools.Deadlines;

namespace SITRA.API.Endpoints
{
    public static class DeadlinesEndpoints
    {
        public static RouteGroupBuilder MapDeadlines(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("deadlines-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("deadlines-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
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
            Guid id)
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
            Guid id, [FromBody] CreateDeadlineDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateDeadlineCommand(id, dto);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateDeadlineCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateDeadlineCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

