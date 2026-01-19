using FluentValidation;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Auths
{
    public class ChangeLanguageDTOValidator : AbstractValidator<ChangeLanguageDTO>
    {
        public ChangeLanguageDTOValidator()
        {
            RuleFor(x => x.Language)
                .IsInEnum().WithMessage(Utilities.InvalidEnumValueMessage);
        }
    }
}
