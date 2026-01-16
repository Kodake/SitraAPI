using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;
using FluentValidation;

namespace SITRA.Core.Validations
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

