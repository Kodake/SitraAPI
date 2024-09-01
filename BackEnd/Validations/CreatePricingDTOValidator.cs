using BackEnd.DTOs.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreatePricingDTOValidator : AbstractValidator<CreatePricingDTO>
    {
        public CreatePricingDTOValidator(IUnitRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (nombre, _) =>
            {
                var existe = await repository.Exists(id, nombre);
                return !existe;
            }).WithMessage(x => $"Ya existe una marca con el nombre {x.Name}");

            RuleFor(x => x.PricingType)
            .IsInEnum().WithMessage("Invalid pricing type specified.");

            RuleFor(x => x.Percentage)
            .NotNull().WithMessage("Percentage is required when PricingType is Percentage.")
            .GreaterThan(0).WithMessage("Percentage must be greater than zero.")
            .When(x => x.PricingType == PricingType.Percentage);
        }
    }
}
