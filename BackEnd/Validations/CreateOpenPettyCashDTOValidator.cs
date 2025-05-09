using BackEnd.DTOs.CashBoxes;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateOpenPettyCashDTOValidator : AbstractValidator<CreateOpenPettyCashDTO>
    {
        public CreateOpenPettyCashDTOValidator()
        {
            RuleFor(x => x.OpeningAmount)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);
        }
    }
}
