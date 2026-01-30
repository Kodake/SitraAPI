using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Attributes;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Mediator.Commands.Accounts.Payables;
using SITRA.Core.Mediator.Queries.Accounts.Payables;

namespace SITRA.API.Endpoints
{
    public static class PayablesEndpoints
    {
        public static RouteGroupBuilder MapPayables(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payables-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetAllById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payables-get")).RequireAuthorization();
            group.MapGet("/{id:guid}/viewPays", GetPaysById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("payables-get")).RequireAuthorization();
            group.MapPost("/{id:guid}/addPayable", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePayableReceiptDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapDelete("/{id:guid}/payableReceipts/{payableReceiptId}/deactivate", Deactivate).RequireAuthorization();
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
            Guid id)
        {
            var query = new GetAllPayablesByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<PayableDTO>, NotFound>> GetPaysById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetPaysByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }



        [Idempotent]
        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePayableReceiptDTO dto,
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new CreatePayableReceiptCommand(id, dto);
            var DTO = await mediator.Send(command);
            return DTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        [Idempotent]
        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromRoute] Guid payableReceiptId,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivatePayableCommand(id, payableReceiptId);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

