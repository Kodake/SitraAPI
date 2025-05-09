using BackEnd.DTOs;
using BackEnd.DTOs.Accounts.PayableReceipts;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Accounts.Payables;
using BackEnd.Mediator.Queries.Accounts.Payables;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class PayablesEndpoints
    {
        public static RouteGroupBuilder MapPayables(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payables-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:long}", GetAllById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payables-get"));
            group.MapGet("/{id:long}/viewPays", GetPaysById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payables-get"));
            group.MapPost("/{id:long}/addPayable", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePayableReceiptDTO>>()
                .WithOpenApi();
            group.MapDelete("/{id:long}/payableReceipts/{payableReceiptId}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<PayableListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllPayablesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<PayableDTO>, NotFound>> GetAllById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetAllPayablesByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<PayableDTO>, NotFound>> GetPaysById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetPaysByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePayableReceiptDTO dto,
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new CreatePayableReceiptCommand(id, dto);
            var DTO = await mediator.Send(command);
            return DTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromRoute] long payableReceiptId,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivatePayableCommand(id, payableReceiptId);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
