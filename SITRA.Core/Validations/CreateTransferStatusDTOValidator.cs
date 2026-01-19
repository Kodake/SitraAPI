using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Interfaces;
using SITRA.Core.Enums;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations
{
    public class CreateTransferStatusDTOValidator : AbstractValidator<CreateTransferStatusDTO>
    {
        public CreateTransferStatusDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;
            if (routeId != null && int.TryParse(routeId.ToString(), out id))
            {
                RuleFor(x => x)
                    .MustAsync(async (dto, cancellation) =>
                    {
                        var transfer = await unitOfWork.TransferRepository.GetByIdAsync(id);
                        if (transfer == null) return true;
                        return transfer.Status != TransferStatus.Cancelled && transfer.Status != TransferStatus.Received;
                    })
                    .WithMessage(Utilities.TransferStatusChangeErrorMessage)
                    .WithName("Status");

                RuleFor(x => x)
                    .MustAsync(async (dto, cancellation) =>
                    {
                        var transfer = await unitOfWork.TransferRepository.GetByIdAsync(id);
                        if (transfer == null) return true;

                        var newStatus = httpContextAccessor.HttpContext.Request.Path.Value.Contains("received") 
                            ? TransferStatus.Received : TransferStatus.Cancelled;

                        var warehouseId = newStatus == TransferStatus.Received 
                            ? transfer.DestinationWarehouseId : transfer.OriginWarehouseId;

                        var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(transfer.ProductId, warehouseId);
                        return stock != null;
                    })
                    .WithMessage((dto) => {
                        var newStatus = httpContextAccessor.HttpContext.Request.Path.Value.Contains("received") 
                            ? "Destination" : "Origin";
                        return string.Format(Utilities.TransferStockNotFoundMessage, newStatus);
                    })
                    .WithName("Stock");
            }

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage(Utilities.TransferIdInvalidMessage);

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage(Utilities.CommentTooLongMessage);
        }
    }
}
