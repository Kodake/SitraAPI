using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Finances;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateCurrencyDTOValidator : AbstractValidator<CreateCurrencyDTO>
    {
        public CreateCurrencyDTOValidator(ICurrencyRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Code)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(20).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (code, _) =>
            {
                var exists = await repository.Exists(id, code);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.CodeAlreadyExistsWith, x.Name));

            RuleFor(x => x.Code)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);

            RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(1).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(3).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);
        }
    }
}


