using FluentValidation;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Products
{
    public class CreateProductCsvDTOValidator : AbstractValidator<CreateProductCsvDTO>
    {
        public CreateProductCsvDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Reference)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Barcode)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.InitialCost)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);
        }
    }
}

