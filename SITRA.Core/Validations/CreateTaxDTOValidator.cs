using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Finances;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateTaxDTOValidator : AbstractValidator<CreateTaxDTO>
    {
        public CreateTaxDTOValidator(IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Amount)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(20).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);
        }
    }
}


