using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
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

        static async Task<Ok<List<TransferListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var transfers = await unitOfWork.TransferRepository.GetAllAsync(pagination);
            var transfersDTO = mapper.Map<List<TransferListDTO>>(transfers);

            return TypedResults.Ok(transfersDTO);
        }

        static async Task<Results<Ok<TransferDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var transfer = await unitOfWork.TransferRepository.GetByIdAsync(id);

            if (transfer is null)
            {
                return TypedResults.NotFound();
            }

            var transferDTO = mapper.Map<TransferDTO>(transfer);

            return TypedResults.Ok(transferDTO);
        }

        static async Task<Results<Ok<TransferDTO>, ValidationProblem>> Create(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper,
            [FromBody] CreateTransferDTO dto,
            [FromServices] ApplicationDbContext dbContext,
            IOutputCacheStore outputCacheStore)
        {
            var stockOrigin = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);
            if (stockOrigin == null || stockOrigin.Qty < dto.Qty)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { stockOrigin == null ? "Origin stock not found." : "Insufficient stock quantity." } }
                });
            }

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.DestinationWarehouseId);

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
                    await unitOfWork.StockRepository.CreateAsync(stockDestination);
                }

                var transfer = mapper.Map<Transfer>(dto);
                transfer.StockId = stockOrigin.Id;
                transfer.Status = TransferStatus.Transit;
                transfer.OriginWarehouseId = dto.OriginWarehouseId;

                stockOrigin.Qty -= dto.Qty;

                await unitOfWork.TransferRepository.CreateAsync(transfer);
                unitOfWork.StockRepository.Update(stockOrigin);
                unitOfWork.StockRepository.Update(stockDestination);
                await unitOfWork.SaveChangesAsync();

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
            [FromBody] CreateTransferStatusDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            var existingTransfer = await unitOfWork.TransferRepository.GetByIdAsync(id);
            if (existingTransfer == null)
                return TypedResults.NotFound();

            if (existingTransfer.Status == TransferStatus.Cancelled || existingTransfer.Status == TransferStatus.Received)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Cannot change the status of this transfer." } }
                });

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(existingTransfer.ProductId, existingTransfer.DestinationWarehouseId);

            if (stockDestination == null)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Destination stock not found." } }
                });

            stockDestination.Qty += existingTransfer.Qty;
            unitOfWork.StockRepository.Update(stockDestination);

            existingTransfer.Status = TransferStatus.Received;
            existingTransfer.UpdatedAt = DateTime.UtcNow;
            unitOfWork.TransferRepository.Update(existingTransfer);

            await unitOfWork.SaveChangesAsync();
            await EvictCacheAsync(outputCacheStore);

            var transferDTO = mapper.Map<TransferDTO>(existingTransfer);
            return TypedResults.Ok(transferDTO);
        }

        static async Task<Results<Ok<TransferDTO>, NotFound, ValidationProblem>> Cancelled(
            int id,
            [FromBody] CreateTransferStatusDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            var existingTransfer = await unitOfWork.TransferRepository.GetByIdAsync(id);
            if (existingTransfer == null)
                return TypedResults.NotFound();

            if (existingTransfer.Status == TransferStatus.Cancelled || existingTransfer.Status == TransferStatus.Received)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Cannot change the status of this transfer." } }
                });

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(existingTransfer.ProductId, existingTransfer.OriginWarehouseId);
            if (stockDestination == null)
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Origin stock not found." } }
                });

            stockDestination.Qty += existingTransfer.Qty;
            unitOfWork.StockRepository.Update(stockDestination);

            existingTransfer.Status = TransferStatus.Cancelled;
            existingTransfer.UpdatedAt = DateTime.UtcNow;
            unitOfWork.TransferRepository.Update(existingTransfer);

            await unitOfWork.SaveChangesAsync();
            await EvictCacheAsync(outputCacheStore);

            var transferDTO = mapper.Map<TransferDTO>(existingTransfer);
            return TypedResults.Ok(transferDTO);
        }

        private static async Task EvictCacheAsync(IOutputCacheStore outputCacheStore)
        {
            var tags = new[] { "transfers-get", "stocks-get", "warehouses-get", "products-get" };
            foreach (var tag in tags)
            {
                await outputCacheStore.EvictByTagAsync(tag, default);
            }
        }
    }
}
