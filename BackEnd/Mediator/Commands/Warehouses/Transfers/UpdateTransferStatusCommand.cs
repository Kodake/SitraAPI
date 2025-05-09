using AutoMapper;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Entities.Warehouses;
using BackEnd.Enums;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Warehouses.Transfers
{
    public class UpdateTransferStatusCommand(long id, CreateTransferStatusDTO dto, TransferStatus newStatus) : UpdateCommand<Transfer, CreateTransferStatusDTO>(id, dto)
    {
        private readonly TransferStatus _newStatus = newStatus;

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var existingTransfer = await unitOfWork.TransferRepository.GetByIdAsync(Id);
            if (existingTransfer == null)
            {
                return false;
            }

            if (existingTransfer.Status == TransferStatus.Cancelled || existingTransfer.Status == TransferStatus.Received)
            {
                throw new InvalidOperationException("Cannot change the status of this transfer.");
            }

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(existingTransfer.ProductId, _newStatus == TransferStatus.Received 
                ? existingTransfer.DestinationWarehouseId : existingTransfer.OriginWarehouseId) 
                ?? throw new InvalidOperationException(_newStatus == TransferStatus.Received ? "Destination stock not found." : "Origin stock not found.");

            stockDestination.Qty += existingTransfer.Qty;
            unitOfWork.StockRepository.Update(stockDestination);

            existingTransfer.Status = _newStatus;
            existingTransfer.UpdatedAt = DateTime.UtcNow;
            unitOfWork.TransferRepository.Update(existingTransfer);

            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("transfers-get", default);
            await outputCacheStore.EvictByTagAsync("stocks-get", default);
            await outputCacheStore.EvictByTagAsync("warehouses-get", default);
            await outputCacheStore.EvictByTagAsync("products-get", default);

            return true;
        }
    }
}
