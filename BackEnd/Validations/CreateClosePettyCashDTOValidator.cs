using BackEnd.DTOs.CashBoxes;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateClosePettyCashDTOValidator : AbstractValidator<CreateClosePettyCashDTO>
    {
        public CreateClosePettyCashDTOValidator()
        {
            RuleFor(x => x.Comments)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}
