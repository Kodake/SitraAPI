using AutoMapper;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;
using System.Transactions;

namespace BackEnd.MediatR.Commands.Warehouses.Transfers
{
    public class CreateTransferCommand : CreateCommand<Transfer, CreateTransferDTO>
    {
        public CreateTransferCommand(CreateTransferDTO dto) : base(dto) { }

        public override async Task<CreateTransferDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var stockOrigin = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(Dto.ProductId, Dto.OriginWarehouseId);
            if (stockOrigin == null || stockOrigin.Qty < Dto.Qty)
            {
                throw new InvalidOperationException(stockOrigin == null ? "Origin stock not found." : "Insufficient stock quantity.");
            }

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(Dto.ProductId, Dto.DestinationWarehouseId);

            if (stockDestination == null)
            {
                stockDestination = new Stock
                {
                    ItemId = Dto.ProductId,
                    WarehouseId = Dto.DestinationWarehouseId,
                    Qty = 0
                };
                await unitOfWork.StockRepository.CreateAsync(stockDestination);
            }

            var transfer = mapper.Map<Transfer>(Dto);
            transfer.StockId = stockOrigin.Id;
            transfer.Status = TransferStatus.Transit;
            transfer.OriginWarehouseId = Dto.OriginWarehouseId;

            stockOrigin.Qty -= Dto.Qty;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await unitOfWork.TransferRepository.CreateAsync(transfer);
                unitOfWork.StockRepository.Update(stockOrigin);
                unitOfWork.StockRepository.Update(stockDestination);
                await unitOfWork.SaveChangesAsync();

                transaction.Complete();
            }

            await outputCacheStore.EvictByTagAsync("transfers-get", default);
            await outputCacheStore.EvictByTagAsync("stocks-get", default);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);
            await outputCacheStore.EvictByTagAsync("products-get", default);

            return Dto;
        }
    }
}
