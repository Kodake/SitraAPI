using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.CashBoxes;
using SITRA.Core.Mediator.Queries.CashBoxes;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SITRA.API.Endpoints.CashBoxes
{
    public static class PettyCashesEndpoints
    {
        public static RouteGroupBuilder MapPettyCashes(this RouteGroupBuilder group)
        {
            group.MapGet("/{pettyCashId:long}/pettyCashMovements", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pettyCashMovements-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pettyCashes-get"))
                .RequireAuthorization();
            group.MapGet("/{pettyCashId:long}/pettyCashPayments", GetAllById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pettyCashes-get"));
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

        static async Task<Ok<List<PaymentMethodAmountListDTO>>> GetAllById(
            [FromServices] IMediator mediator,
            [FromRoute] long pettyCashId)
        {
            var query = new GetAllPaymentMethodSummaryByIdQuery(pettyCashId);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateOpenPettyCashDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateOpenPettyCashCommand(dto);
            var pettyCashDTO = await mediator.Send(command);
            return pettyCashDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<FileContentHttpResult, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateClosePettyCashDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateClosePettyCashCommand(id, dto);
            var result = await mediator.Send(command);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }
    }
}

