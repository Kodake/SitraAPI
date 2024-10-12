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
using Microsoft.EntityFrameworkCore;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Cryptography.Xml;
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
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
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

        static async Task<Results<Created<TransferDTO>, ValidationProblem>> Create(
            [FromBody] CreateTransferDTO dto,
            [FromServices] ITransferRepository repository,
            [FromServices] IStockRepository stockRepository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext dbContext)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var transfer = mapper.Map<Transfer>(dto);

                var stockOrigin = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);
                var stockDestination = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.DestinationWarehouseId);

                if (stockOrigin == null)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Origin stock not found." } }
                    });
                }

                if (stockDestination == null)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Destination stock not found." } }
                    });
                }

                switch (dto.Status)
                {
                    case TransferStatus.Received:
                        stockOrigin.Qty -= dto.Qty;
                        await stockRepository.Update(stockOrigin);

                        if (stockDestination != null)
                        {
                            stockDestination.Qty += dto.Qty;
                            await stockRepository.Update(stockDestination);
                        }
                        else
                        {
                            var newStock = new Stock
                            {
                                ItemId = dto.ProductId,
                                WarehouseId = dto.DestinationWarehouseId,
                                Qty = dto.Qty
                            };
                            await stockRepository.Create(newStock);
                        }
                        break;

                    case TransferStatus.Transit:
                        stockOrigin.Qty -= dto.Qty;
                        await stockRepository.Update(stockOrigin);
                        break;

                    case TransferStatus.Cancelled:
                        break;

                    default:
                        return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                        {
                            { "Error", new[] { "Invalid transfer status." } }
                        });
                }

                transfer.StockId = stockOrigin.Id;

                var id = await repository.Create(transfer);
                await transaction.CommitAsync();

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("warehouses-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);

                var transferDTO = mapper.Map<TransferDTO>(transfer);
                return TypedResults.Created($"/transfers/{id}", transferDTO);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while creating the transfer." } }
                });
            }
        }

        static async Task<Results<Ok<TransferDTO>, NotFound, ValidationProblem>> Update(
            int id,
            [FromBody] CreateTransferDTO dto,
            [FromServices] ITransferRepository repository,
            [FromServices] IStockRepository stockRepository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext dbContext)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var existingTransfer = await repository.GetById(id);
                if (existingTransfer == null)
                {
                    return TypedResults.NotFound();
                }

                var stockOrigin = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);
                var stockDestination = await stockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.DestinationWarehouseId);

                if (stockOrigin == null || stockDestination == null)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { stockOrigin == null ? "Origin stock not found." : "Destination stock not found." } }
                    });
                }

                if (existingTransfer.Status == TransferStatus.Cancelled ||
                    (dto.Status == TransferStatus.Cancelled && existingTransfer.Status == TransferStatus.Received) ||
                    (dto.Status == TransferStatus.Transit))
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Cannot change the status of this transfer." } }
                    });
                }

                switch (dto.Status)
                {
                    case TransferStatus.Received:
                        stockDestination.Qty += existingTransfer.Qty;
                        await stockRepository.Update(stockDestination);
                        break;

                    case TransferStatus.Cancelled:
                        stockOrigin.Qty += existingTransfer.Qty;
                        await stockRepository.Update(stockOrigin);
                        break;
                }

                mapper.Map(dto, existingTransfer);
                existingTransfer.Status = dto.Status;

                await repository.Update(existingTransfer);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);
                await outputCacheStore.EvictByTagAsync("warehouses-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);

                var transferDTO = mapper.Map<TransferDTO>(existingTransfer);
                return TypedResults.Ok(transferDTO);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while updating the transfer: " + ex.Message } }
                });
            }
        }
    }
}
