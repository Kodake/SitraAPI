using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Enums;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Warehouses.Transfers;
using SITRA.Core.Mediator.Queries.Warehouses.Transfers;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Attributes;

namespace SITRA.API.Endpoints
{
    public static class TransfersEndpoints
    {
        public static RouteGroupBuilder MapTransfers(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("transfers-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferDTO>>()
               .WithOpenApi().RequireAuthorization();
            group.MapPut("/received/{id:guid}", Received).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferStatusDTO>>()
               .WithOpenApi().RequireAuthorization();
            group.MapPut("/cancelled/{id:guid}", Cancelled).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferStatusDTO>>()
               .WithOpenApi().RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<TransferListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllTransfersQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<TransferDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetTransferByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateTransferDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateTransferCommand(dto);
            var transferDTO = await mediator.Send(command);
            return transferDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }



        [Idempotent]
        static async Task<Results<Ok, NotFound, ValidationProblem>> Received(
            Guid id,
            [FromBody] CreateTransferStatusDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateTransferStatusCommand(id, dto, TransferStatus.Received);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        [Idempotent]
        static async Task<Results<Ok, NotFound, ValidationProblem>> Cancelled(
            Guid id,
            [FromBody] CreateTransferStatusDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateTransferStatusCommand(id, dto, TransferStatus.Cancelled);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

