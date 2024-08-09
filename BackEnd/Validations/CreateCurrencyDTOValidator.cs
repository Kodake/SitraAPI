using BackEnd.DTOs.Currencies;
using BackEnd.DTOs.Products;
using BackEnd.Helpers;
using BackEnd.Repositories.Currencies;
using BackEnd.Repositories.Products;
using FluentValidation;

namespace BackEnd.Validations
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
                var existe = await repository.Exists(id, code);
                return !existe;
            }).WithMessage(x => $"Ya existe una moneda con el código {x.Code}");

            RuleFor(x => x.Code)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);
        }
    }
}
