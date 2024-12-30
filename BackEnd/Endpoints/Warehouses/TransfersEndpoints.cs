using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.Warehouses.Transfers;
using BackEnd.MediatR.Queries.Warehouses.Transfers;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class TransfersEndpoints
    {
        public static RouteGroupBuilder MapTransfers(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("transfers-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferDTO>>()
               .WithOpenApi();
            group.MapPut("/received/{id:int}", Received).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferDTO>>()
               .WithOpenApi();
            group.MapPut("/cancelled/{id:int}", Cancelled).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferDTO>>()
               .WithOpenApi();
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
            int id)
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

        static async Task<Results<Ok, NotFound, ValidationProblem>> Received(
            int id,
            [FromBody] CreateTransferStatusDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateTransferStatusCommand(id, dto, TransferStatus.Received);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Cancelled(
            int id,
            [FromBody] CreateTransferStatusDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateTransferStatusCommand(id, dto, TransferStatus.Cancelled);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
