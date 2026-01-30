using FluentValidation;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Auths
{
    public class CreateRegistrationUserDTOValidator
        : AbstractValidator<CreateRegistrationUserDTO>
    {
        public CreateRegistrationUserDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .EmailAddress().WithMessage(Utilities.EmailMessage);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(6).WithMessage(Utilities.PasswordMinLengthMessage);

            RuleFor(x => x.Address)
                .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                    .WithMessage(Utilities.RequiredFieldMessage)
                .Matches(@"^\d{10}$")
                    .WithMessage(Utilities.PhoneNumberFieldMessage);

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.RucNumber)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .Matches(@"^\d{9}(\d{2})?$")
                .WithMessage(Utilities.RucFieldMessage);

            RuleFor(x => x.Plan)
                .IsInEnum().WithMessage("Invalid Plan Type.");
        }
    }
}


