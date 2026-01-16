using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Helpers;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateServiceCsvDTOValidator : AbstractValidator<CreateServiceCsvDTO>
    {
        public CreateServiceCsvDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Reference)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Description)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);
        }
    }
}

