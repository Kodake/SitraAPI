using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Enums;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Warehouses.Warehouses;
using SITRA.Core.Mediator.Queries.Warehouses.Warehouses;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.API.Endpoints
{
    public static class WarehousesEndpoints
    {
        public static RouteGroupBuilder MapWarehouses(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/stock-warehouses/{id:long}", GetWarehouseStockByProductId).RequireAuthorization();
            group.MapGet("/unregistered-warehouses/{id:long}", GetUnregisteredWarehouseById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get")).RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("warehouses-get")).RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWarehouseDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateWarehouseDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:long}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:long}/deactivate", Deactivate).RequireAuthorization();
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
            long id)
        {
            var query = new GetUnregisteredWarehouseByIdQuery(id);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<WarehouseListDTO>>> GetWarehouseStockByProductId(
            [FromServices] IMediator mediator,
            long id)
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
            long id)
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
            long id,
            [FromBody] CreateWarehouseDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateWarehouseCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateWarehouseCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateWarehouseCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

