using BackEnd.DTOs.Products.Combos;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateComboDTOValidator : AbstractValidator<CreateComboDTO>
    {
        public CreateComboDTOValidator()
        {
            RuleFor(x => x.ItemIds)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .Must(ids => ids.Count >= 2)
                    .WithMessage(Utilities.MinItemsRequired) // Actualizado
                .Must(ids => ids.Count <= 50)
                    .WithMessage(Utilities.MaximunLengthMessage)
                .Must(ids => ids.All(id => id > 0))
                    .WithMessage(Utilities.EachItemMustBeGreatherThanZero); // Actualizado

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
        }
    }
}