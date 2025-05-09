using BackEnd.DTOs;
using BackEnd.Filters;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Mediator.Queries.CashBoxes;
using Microsoft.AspNetCore.OutputCaching;
using BackEnd.Mediator.Commands.CashBoxes;
using BackEnd.Helpers;

namespace BackEnd.Endpoints.CashBoxes
{
    public static class PettyCashesEndpoints
    {
        public static RouteGroupBuilder MapPettyCashes(this RouteGroupBuilder group)
        {
            group.MapGet("/{pettyCashId:long}/pettyCashMovements", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pettyCashMovements-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pettyCashes-get"))
                .RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateOpenPettyCashDTO>>()
                .RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateClosePettyCashDTO>>()
                .RequireAuthorization()
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<PettyCashMovementListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            [FromRoute] long pettyCashId,
            PaginationDTO pagination)
        {
            var query = new GetAllPettyCashMovementsByIdQuery(pagination, pettyCashId);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<PettyCashDTO>> GetById(
            [FromServices] IMediator mediator)
        {
            var query = new GetPettyCashByUserIdQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateOpenPettyCashDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateOpenPettyCashCommand(dto);
            var contactDTO = await mediator.Send(command);
            return contactDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateClosePettyCashDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateClosePettyCashCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
