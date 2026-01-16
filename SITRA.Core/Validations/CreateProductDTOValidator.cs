using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Reference)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.InitialCost)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            // Validate warehouse exists if WarehouseId is provided
            RuleFor(x => x.WarehouseId)
                .MustAsync(async (warehouseId, cancellation) =>
                {
                    if (warehouseId == null) return true; // Optional field

                    var warehouse = await unitOfWork.WarehouseRepository.GetByIdAsync(warehouseId.Value);
                    return warehouse != null;
                })
                .WithMessage("Warehouse with ID {PropertyValue} not found.")
                .When(x => x.WarehouseId.HasValue);
        }
    }
}

