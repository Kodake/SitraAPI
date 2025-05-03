using BackEnd.DTOs.Auths;
using BackEnd.Entities.Auths;
using BackEnd.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Validations
{
    public class CreateRegistrationUserDTOValidator
        : AbstractValidator<CreateRegistrationUserDTO>
    {
        public CreateRegistrationUserDTOValidator(UserManager<User> repository,
          IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}