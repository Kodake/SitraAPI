using BackEnd.DTOs;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.CashBoxes;
using BackEnd.Mediator.Queries.CashBoxes;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints.CashBoxes
{
    public static class PettyCashMovementsEndpoints
    {
        public static RouteGroupBuilder MapPettyCashMovements(this RouteGroupBuilder group)
        {
            group.MapGet("/{pettyCashId:long}/withdrawals", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pettyCashMovements-get"))
                .AddPaginationParamsOpenAPI();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWithdrawalPettyCashMovementDTO>>()
                .RequireAuthorization()
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<WithdrawalPettyCashMovementListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            [FromRoute] long pettyCashId,
            PaginationDTO pagination)
        {
            var query = new GetAllWithdrawalPettyCashMovementsByIdQuery(pagination, pettyCashId);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateWithdrawalPettyCashMovementDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateWithdrawalPettyCashMovementCommand(dto);
            var movementDTO = await mediator.Send(command);
            return movementDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }
    }
}
