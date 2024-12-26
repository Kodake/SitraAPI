using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.AdministrativeTools.FiscalSupports;
using BackEnd.MediatR.Queries.AdministrativeTools.FiscalSupports;
using BackEnd.MediatR.Queries.Products.Items;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class FiscalSupportsEndpoints
    {
        public static RouteGroupBuilder MapFiscalSupports(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscalsupports-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscalsupports-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/supportType/{supportType}/warehouse/{warehouseId:int}", GetByDocumentTypeAndWarehouseId);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<FiscalSupportListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllFiscalSupportsQuery(pagination);
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }

        static async Task<Ok<List<FiscalSupportListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsFiscalSupportsQuery(status);
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }

        static async Task<Results<Ok<FiscalSupportDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            int id)
        {
            var query = new GetFiscalSupportByIdQuery(id);
            var result = await mediator.Send(query);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }

        static async Task<Ok<List<FiscalSupportDTO>>> GetByDocumentTypeAndWarehouseId(
            [FromServices] IMediator mediator,
            [FromRoute] FiscalSupportType supportType, int warehouseId)
        {
            var query = new GetDocumentsByTypeIdAndWarehouseIdQuery(supportType, warehouseId);
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateFiscalSupportDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateFiscalSupportCommand(dto);
            var fiscalSupportDTO = await mediator.Send(command);
            return fiscalSupportDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            int id, [FromBody] CreateFiscalSupportDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateFiscalSupportCommand(id, dto);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateFiscalSupportCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateFiscalSupportCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
