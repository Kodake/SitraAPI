using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.Warehouses;
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
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateStockDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateStockDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<StockListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var stocks = await unitOfWork.StockRepository.GetAllAsync(pagination);
            var stocksDTO = mapper.Map<List<StockListDTO>>(stocks);

            return TypedResults.Ok(stocksDTO);
        }

        static async Task<Results<Ok<StockDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var stock = await unitOfWork.StockRepository.GetByIdAsync(id);

            if (stock is null)
            {
                return TypedResults.NotFound();
            }

            var stockDTO = mapper.Map<StockDTO>(stock);

            return TypedResults.Ok(stockDTO);
        }

        static async Task<Results<Created<StockDTO>, ValidationProblem>> Create(
            [FromBody] CreateStockDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(dto.ItemId, dto.WarehouseId);

            if (stock != null)
            {
                stock.Qty += dto.Qty;
                unitOfWork.StockRepository.Update(stock);
            }
            else
            {
                stock = mapper.Map<Stock>(dto);
                await unitOfWork.StockRepository.CreateAsync(stock);
            }

            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stocks-get", default);

            var stockDTO = mapper.Map<StockDTO>(stock);

            return TypedResults.Created($"/stocks/{stockDTO.Id}", stockDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateStockDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var stockDB = await unitOfWork.StockRepository.GetByIdAsync(id);

            if (stockDB is null)
            {
                return TypedResults.NotFound();
            }

            stockDB.Qty = dto.Qty;
            stockDB.WarehouseId = dto.WarehouseId;

            unitOfWork.StockRepository.Update(stockDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stocks-get", default);

            return TypedResults.Ok();
        }
    }
}
