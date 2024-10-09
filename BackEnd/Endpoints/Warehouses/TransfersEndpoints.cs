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

        //static async Task<Results<Created<TransferDTO>, ValidationProblem>> Create([FromBody] CreateTransferDTO dto,
        //   [FromServices] ITransferRepository repository,
        //   IOutputCacheStore outputCacheStore,
        //   [FromServices] IMapper mapper)
        //{
        //    var transfer = mapper.Map<Transfer>(dto);

        //    var id = await repository.Create(transfer);
        //    await outputCacheStore.EvictByTagAsync("transfers-get", default);
        //    var transferDTO = mapper.Map<TransferDTO>(transfer);
        //    return TypedResults.Created($"/transfers/{id}", transferDTO);
        //}

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

                if (dto.Status == TransferStatus.Cancelled && transfer.Status == TransferStatus.Received)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Cannot cancel a completed transfer." } }
                    });
                }

                switch (dto.Status)
                {
                    case TransferStatus.Received:
                        if (stockOrigin != null)
                        {
                            stockOrigin.Qty -= dto.Qty;
                            await stockRepository.Update(stockOrigin);
                        }
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
                        if (stockOrigin != null)
                        {
                            stockOrigin.Qty -= dto.Qty;
                            await stockRepository.Update(stockOrigin);
                        }
                        break;

                    case TransferStatus.Pending:
                        break;

                    case TransferStatus.Cancelled:
                        break;
                }

                if (stockOrigin != null)
                {
                    transfer.StockId = stockOrigin.Id;
                }
                else
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Origin stock not found." } }
                    });
                }

                var id = await repository.Create(transfer);

                await transaction.CommitAsync();

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);

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
                // Get the existing transfer by id
                var existingTransfer = await repository.GetById(id);
                if (existingTransfer == null)
                {
                    return TypedResults.NotFound();
                }

                // If trying to cancel a completed transfer (status = Received)
                if (dto.Status == TransferStatus.Cancelled && existingTransfer.Status == TransferStatus.Received)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Cannot cancel a completed transfer." } }
                    });
                }

                // Fetch stocks with AsNoTracking to avoid tracking multiple instances
                var stockOrigin = await stockRepository.GetByItemIdAndWarehouseId(existingTransfer.ProductId, existingTransfer.OriginWarehouseId);
                var stockDestination = await stockRepository.GetByItemIdAndWarehouseId(existingTransfer.ProductId, existingTransfer.DestinationWarehouseId);

                // Adjust stock based on the current transfer status
                switch (existingTransfer.Status)
                {
                    case TransferStatus.Received:
                        if (stockOrigin != null) stockOrigin.Qty += existingTransfer.Qty;
                        if (stockDestination != null) stockDestination.Qty -= existingTransfer.Qty;
                        break;

                    case TransferStatus.Transit:
                        if (stockOrigin != null) stockOrigin.Qty += existingTransfer.Qty;
                        break;
                }

                // Update stocks based on the new status
                switch (dto.Status)
                {
                    case TransferStatus.Received:
                        if (stockOrigin != null)
                        {
                            stockOrigin.Qty -= existingTransfer.Qty; // Adjust for the new Received status
                            dbContext.Entry(stockOrigin).State = EntityState.Modified; // Track the modified entity
                        }
                        if (stockDestination != null)
                        {
                            stockDestination.Qty += existingTransfer.Qty; // Adjust for the new Received status
                            dbContext.Entry(stockDestination).State = EntityState.Modified; // Track the modified entity
                        }
                        else
                        {
                            var newStock = new Stock
                            {
                                ItemId = existingTransfer.ProductId,
                                WarehouseId = existingTransfer.DestinationWarehouseId,
                                Qty = existingTransfer.Qty
                            };
                            await stockRepository.Create(newStock);
                        }
                        break;

                    case TransferStatus.Transit:
                        if (stockOrigin != null)
                        {
                            stockOrigin.Qty -= existingTransfer.Qty; // Adjust for the new Transit status
                            dbContext.Entry(stockOrigin).State = EntityState.Modified; // Track the modified entity
                        }
                        break;

                    case TransferStatus.Pending:
                    case TransferStatus.Cancelled:
                        // No stock changes needed for Pending or Cancelled
                        break;
                }

                // Update only the status of the transfer
                existingTransfer.Status = dto.Status;
                await repository.Update(existingTransfer); // Update the transfer status

                await transaction.CommitAsync();

                await outputCacheStore.EvictByTagAsync("transfers-get", default);
                await outputCacheStore.EvictByTagAsync("stocks-get", default);

                var transferDTO = mapper.Map<TransferDTO>(existingTransfer);
                return TypedResults.Ok(transferDTO);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            { "Error", new[] { "An error occurred while updating the transfer." } }
        });
            }
        }
    }
}
