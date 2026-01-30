using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Attributes;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Mediator.Commands.Accounts.Receivables;
using SITRA.Core.Mediator.Queries.Receivables;

namespace SITRA.API.Endpoints
{
    public static class ReceivablesEndpoints
    {
        public static RouteGroupBuilder MapReceivables(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetAllById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get")).RequireAuthorization();
            group.MapGet("/{id:guid}/viewPays", GetPaysById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get")).RequireAuthorization();
            group.MapPost("/{id:guid}/addPayment", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePaymentReceiptDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapDelete("/{id:guid}/paymentReceipts/{paymentReceiptId}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ReceivableListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllReceivablesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ReceivableDTO>, NotFound>> GetAllById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetAllReceivablesByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ReceivableDTO>, NotFound>> GetPaysById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetPaysByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }



        [Idempotent]
        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePaymentReceiptDTO dto,
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new CreatePaymentReceiptCommand(id, dto);
            var DTO = await mediator.Send(command);
            return DTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        [Idempotent]
        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromRoute] Guid paymentReceiptId,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateReceivableCommand(id, paymentReceiptId);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

