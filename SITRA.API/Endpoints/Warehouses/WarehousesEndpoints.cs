using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Warehouses.Warehouses;
using SITRA.Core.Mediator.Queries.Warehouses.Warehouses;

namespace SITRA.API.Endpoints
{
    public static class WarehousesEndpoints
    {
        public static RouteGroupBuilder MapWarehouses(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/stock-warehouses/{id:guid}", GetWarehouseStockByProductId).RequireAuthorization();
            group.MapGet("/unregistered-warehouses/{id:guid}", GetUnregisteredWarehouseById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get")).RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWarehouseDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWarehouseDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapPatch("/{id:guid}/set-default", SetDefault).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllWarehousesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetUnregisteredWarehouseById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetUnregisteredWarehouseByIdQuery(id);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetWarehouseStockByProductId(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetWarehouseStockByProductIdQuery(id);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsWarehousesQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<WarehouseDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetWarehousesByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateWarehouseDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateWarehouseCommand(dto);
            var warehouseDTO = await mediator.Send(command);
            return warehouseDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            Guid id,
            [FromBody] CreateWarehouseDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateWarehouseCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateWarehouseCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateWarehouseCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }


        static async Task<Results<NotFound, Ok>> SetDefault(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new SetDefaultWarehouseCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

