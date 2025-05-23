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
                .GreaterThanOrEqualTo(0).WithMessage(Utilities.GreaterThanOrEqualToZero);
        }
    }
}
