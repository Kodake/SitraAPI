using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateLoginUserDTOValidator : AbstractValidator<CreateLoginUserDTO>
    {
        public CreateLoginUserDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .EmailAddress().WithMessage(Utilities.EmailMessage);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);
        }
    }
}
