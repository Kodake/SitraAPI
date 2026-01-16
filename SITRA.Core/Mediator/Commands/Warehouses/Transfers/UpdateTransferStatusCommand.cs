using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Warehouses.Transfers
{
    public class UpdateTransferStatusCommand(long id, CreateTransferStatusDTO dto, TransferStatus newStatus) : UpdateCommand<Transfer, CreateTransferStatusDTO>(id, dto)
    {
        private readonly TransferStatus _newStatus = newStatus;

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            // Note: These validations are context-dependent (require transferId and newStatus at runtime)
            // and cannot be moved to FluentValidation which validates DTOs at request time
            var existingTransfer = await unitOfWork.TransferRepository.GetByIdAsync(Id);
            if (existingTransfer == null)
            {
                return false;
            }

            var stockDestination = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(existingTransfer.ProductId, _newStatus == TransferStatus.Received
                ? existingTransfer.DestinationWarehouseId : existingTransfer.OriginWarehouseId);

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
