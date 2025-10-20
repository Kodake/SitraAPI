using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Warehouses.Stocks;
using BackEnd.Mediator.Queries.Warehouses.Stocks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class StocksEndpoints
    {
        public static RouteGroupBuilder MapStocks(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("stocks-get"))
                .AddPaginationParamsOpenAPI();
            group.MapPost("/stocks-range", GetAllByWarehousesIds).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("stocks-get"))
                .AddPaginationParamsOpenAPI();
            group.MapPost("/stocks-report", GenerateReportPdf).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("stocks-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateStockDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateStockDTO>>()
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<StockListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllStocksQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<StockDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetStockByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateStockDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateStockCommand(dto);
            var stockDTO = await mediator.Send(command);
            return stockDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateStockDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateStockCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<FileContentHttpResult, NotFound>> GenerateReportPdf(
            [FromBody] List<long> warehouseIds,
            [FromServices] IMediator mediator)
        {
            var query = new GetStockReportPdfQuery(warehouseIds);
            var result = await mediator.Send(query);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }

        static async Task<Ok<List<StockListDTO>>> GetAllByWarehousesIds(
            [FromBody] List<long> warehouseIds,
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllStocksByWarehouseIdsQuery(warehouseIds, pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }
    }
}
