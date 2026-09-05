using FluentValidation;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Warehouses
{
    public class CreateTransferStatusDTOValidator : AbstractValidator<CreateTransferStatusDTO>
    {
        public CreateTransferStatusDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;
            if (routeId != null && Guid.TryParse(routeId.ToString(), out id))
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
                    .WithMessage((dto) =>
                    {
                        var newStatus = httpContextAccessor.HttpContext.Request.Path.Value.Contains("received")
                            ? "Destination" : "Origin";
                        return string.Format(Utilities.TransferStockNotFoundMessage, newStatus);
                    })
                    .WithName("Stock");
            }

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(Utilities.TransferIdInvalidMessage);

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage(Utilities.CommentTooLongMessage);
        }
    }
}
