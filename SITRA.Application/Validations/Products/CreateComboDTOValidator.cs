using FluentValidation;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Products
{
    public class CreateComboDTOValidator : AbstractValidator<CreateComboDTO>
    {
        public CreateComboDTOValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .Must(items => items.Count >= 2)
                    .WithMessage(Utilities.MinItemsRequired)
                .Must(items => items.Count <= 50)
                    .WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(500).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Reference)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.InitialCost)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.ImageUrl)
                .ValidImage(required: false);
        }
    }
}

