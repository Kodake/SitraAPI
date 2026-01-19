using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateTransferDTOValidator : AbstractValidator<CreateTransferDTO>
    {
        public CreateTransferDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.OriginWarehouseId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.DestinationWarehouseId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.Qty)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            // Validate origin stock exists
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var stockOrigin = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);
                    return stockOrigin != null;
                })
                .WithMessage(Utilities.OriginStockNotFoundMessage)
                .WithName("OriginStock");

            // Validate sufficient stock quantity
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var stockOrigin = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(dto.ProductId, dto.OriginWarehouseId);
                    if (stockOrigin == null) return true; // Skip if stock validation already failed

                    return stockOrigin.Qty >= dto.Qty;
                })
                .WithMessage(Utilities.InsufficientStockQuantityMessage)
                .WithName("StockQuantity");
        }
    }
}
