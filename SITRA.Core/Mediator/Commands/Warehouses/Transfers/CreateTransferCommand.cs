using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using System.Transactions;

namespace SITRA.Core.Mediator.Commands.Warehouses.Transfers
{
    public class CreateTransferCommand : CreateCommand<Transfer, CreateTransferDTO>
    {
        public CreateTransferCommand(CreateTransferDTO dto) : base(dto) { }

        public override async Task<CreateTransferDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // Validation ensures stock exists and has sufficient quantity
            var stockOrigin = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(Dto.ProductId, Dto.OriginWarehouseId);

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(Dto.ProductId, Dto.DestinationWarehouseId);

            if (stockDestination == null)
            {
                stockDestination = new Stock
                {
                    ItemId = Dto.ProductId,
                    WarehouseId = Dto.DestinationWarehouseId,
                    Qty = 0
                };

            }

            var transfer = mapper.Map<Transfer>(Dto);
            transfer.StockId = stockOrigin!.Id;
            transfer.Status = TransferStatus.Transit;
            transfer.OriginWarehouseId = Dto.OriginWarehouseId;

            stockOrigin.Qty -= Dto.Qty;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await unitOfWork.StockRepository.CreateAsync(stockDestination);
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
