using FluentValidation;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Finances;

namespace SITRA.Core.Validations.AdministrativeTools
{
    public class CreateCurrencyDTOValidator : AbstractValidator<CreateCurrencyDTO>
    {
        public CreateCurrencyDTOValidator(ICurrencyRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
                .MaximumLength(20).WithMessage(Utilities.MaximunLengthMessage)
                .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
                .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
                .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(1).WithMessage(Utilities.MinimunLengthMessage)
                .MaximumLength(3).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Rate)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            if (id == Guid.Empty)
            {
                RuleFor(x => x.Code)
                    .MustAsync(async (code, cancellation) =>
                    {
                        var exists = await repository.Exists(id, code, cancellation);
                        return !exists;
                    }).WithMessage(x => string.Format(Utilities.CodeAlreadyExistsWith, x.Name));
            }
            else
            {
                RuleFor(x => x.Code)
                    .MustAsync(async (code, cancellation) =>
                    {
                        var existing = await repository.GetByIdAsync(id, cancellation);
                        return existing == null || existing.Code == code;
                    })
                    .WithMessage(Utilities.CurrencyCodeCannotBeModifiedMessage);

                RuleFor(x => x.Name)
                    .MustAsync(async (name, cancellation) =>
                    {
                        var existing = await repository.GetByIdAsync(id, cancellation);
                        return existing == null || existing.Name == name;
                    })
                    .WithMessage(Utilities.CurrencyNameCannotBeModifiedMessage);
            }
        }
    }
}



