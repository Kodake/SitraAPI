using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;
using FluentValidation;

namespace SITRA.Core.Validations
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

