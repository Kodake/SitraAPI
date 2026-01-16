using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateRegistrationUserDTOValidator
        : AbstractValidator<CreateRegistrationUserDTO>
    {
        public CreateRegistrationUserDTOValidator(IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                _ = int.TryParse(valorString, out id);
            }

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
        }
    }
}


