using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Enums;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Queries.Products.Items;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.API.Endpoints
{
    public static class FiscalSupportsEndpoints
    {
        public static RouteGroupBuilder MapFiscalSupports(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscalsupports-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscalsupports-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapGet("/supportType/{supportType}/warehouse/{warehouseId:long}", GetByDocumentTypeAndWarehouseId);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:long}/activate", Activate);
            group.MapDelete("/{id:long}/deactivate", Deactivate);
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
            long id)
        {
            var query = new GetFiscalSupportByIdQuery(id);
            var result = await mediator.Send(query);
            return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
        }

        static async Task<Ok<FiscalSupportDTO>> GetByDocumentTypeAndWarehouseId(
            [FromServices] IMediator mediator,
            [FromRoute] FiscalSupportType supportType, long warehouseId)
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
            long id, [FromBody] CreateFiscalSupportDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateFiscalSupportCommand(id, dto);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateFiscalSupportCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateFiscalSupportCommand(id);
            var result = await mediator.Send(command);
            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

