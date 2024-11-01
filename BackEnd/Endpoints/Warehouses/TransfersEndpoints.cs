using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Warehouses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Transactions;

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
               //.RequireAuthorization()
               .WithOpenApi();
            group.MapPut("/received/{id:int}", Received).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferDTO>>()
               //.RequireAuthorization()
               .WithOpenApi();
            ////.RequireAuthorization("isAdmin");
            group.MapPut("/cancelled/{id:int}", Cancelled).DisableAntiforgery()
               .AddEndpointFilter<ValidationFilter<CreateTransferDTO>>()
               //.RequireAuthorization()
               .WithOpenApi();
            ////.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<TransferListDTO>>> GetAll([FromServices] ITransferRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var transfers = await repository.GetAll(pagination);
            var transfersDTO = mapper.Map<List<TransferListDTO>>(transfers);
            return TypedResults.Ok(transfersDTO);
        }

        static async Task<Results<Ok<TransferDTO>, NotFound>> GetById([FromServices] ITransferRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var transfer = await repository.GetById(id);

            if (transfer is null)
            {
                return TypedResults.NotFound();
            }

            var transferDTO = mapper.Map<TransferDTO>(transfer);
            return TypedResults.Ok(transferDTO);
        }

        static async Task<Results<Ok<TransferDTO>, ValidationProblem>> Create(
            [FromServices] ITransferRepository transferRepository,
            [FromServices] IStockRepository stockRepository,
            [FromServices] IMapper mapper,
            [FromBody] CreateTransferDTO dto,
            [FromServices] ApplicationDbContext dbContext,
            IOutputCacheStore outputCacheStore)
        {
            var stockOrigin = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);
            if (stockOrigin == null || stockOrigin.Qty < dto.Qty)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { stockOrigin == null ? "Origin stock not found." : "Insufficient stock quantity." } }
                });
            }

            var stockDestination = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.DestinationWarehouseId);

            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                if (stockDestination == null)
                {
                    stockDestination = new Stock
                    {
                        ItemId = dto.ProductId,
                        WarehouseId = dto.DestinationWarehouseId,
                        Qty = 0
                    };
                    await stockRepository.Create(stockDestination);
                }

                var transfer = mapper.Map<Transfer>(dto);
                transfer.StockId = stockOrigin.Id;
                transfer.Status = TransferStatus.Transit;
                transfer.OriginWarehouseId = dto.OriginWarehouseId;

                stockOrigin.Qty -= dto.Qty;

                await transferRepository.Create(transfer);
                await stockRepository.Update(stockOrigin);
                await stockRepository.Update(stockDestination);

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("warehouses-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);
                transaction.Complete();

                var transferDTO = mapper.Map<TransferDTO>(transfer);
                return TypedResults.Ok(transferDTO);
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"Transaction failed: {ex.Message}" } }
                });
            }
        }

        static async Task<Results<Ok<TransferDTO>, NotFound, ValidationProblem>> Received(
            int id,
            [FromBody] CreateTransferDTO dto,
            [FromServices] ITransferRepository repository,
            [FromServices] IStockRepository stockRepository,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext dbContext,
            IOutputCacheStore outputCacheStore)
        {
            var existingTransfer = await repository.GetById(id);
            if (existingTransfer == null)
            {
                return TypedResults.NotFound();
            }

            var stockDestination = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.DestinationWarehouseId);

            if (stockDestination == null)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Destination stock not found." } }
                });
            }

            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                if (existingTransfer.Status == TransferStatus.Cancelled ||
                    (dto.Status == TransferStatus.Cancelled && existingTransfer.Status == TransferStatus.Received) ||
                    (dto.Status == TransferStatus.Transit))
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Cannot change the status of this transfer." } }
                    });
                }

                if (dto.Status == TransferStatus.Received)
                {
                    var stockDTO = mapper.Map<StockDTO>(stockDestination);
                    stockDTO.Qty += existingTransfer.Qty;
                    var stock = mapper.Map<Stock>(stockDTO);
                    await stockRepository.Update(stock);
                }

                existingTransfer.UpdatedAt = DateTime.UtcNow;
                existingTransfer.Status = dto.Status;

                await repository.Update(existingTransfer);

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("warehouses-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);
                transaction.Complete();

                var transferDTO = mapper.Map<TransferDTO>(existingTransfer);
                return TypedResults.Ok(transferDTO);
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"Transaction failed: {ex.Message}" } }
                });
            }
        }

        static async Task<Results<Ok<TransferDTO>, NotFound, ValidationProblem>> Cancelled(
            int id,
            [FromBody] CreateTransferDTO dto,
            [FromServices] ITransferRepository repository,
            [FromServices] IStockRepository stockRepository,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext dbContext,
            IOutputCacheStore outputCacheStore)
        {
            var existingTransfer = await repository.GetById(id);
            if (existingTransfer == null)
            {
                return TypedResults.NotFound();
            }

            var stockDestination = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);

            if (stockDestination == null)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Destination stock not found." } }
                });
            }

            try
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                if (existingTransfer.Status == TransferStatus.Cancelled ||
                    (dto.Status == TransferStatus.Cancelled && existingTransfer.Status == TransferStatus.Received) ||
                    (dto.Status == TransferStatus.Transit))
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Cannot change the status of this transfer." } }
                    });
                }

                if (dto.Status == TransferStatus.Cancelled)
                {
                    var stockDTO = mapper.Map<StockDTO>(stockDestination);
                    stockDTO.Qty += existingTransfer.Qty;
                    var stock = mapper.Map<Stock>(stockDTO);
                    await stockRepository.Update(stock);
                }

                existingTransfer.UpdatedAt = DateTime.UtcNow;
                existingTransfer.Status = dto.Status;

                await repository.Update(existingTransfer);

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("warehouses-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);
                transaction.Complete();

                var transferDTO = mapper.Map<TransferDTO>(existingTransfer);
                return TypedResults.Ok(transferDTO);
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"Transaction failed: {ex.Message}" } }
                });
            }
        }
    }
}
