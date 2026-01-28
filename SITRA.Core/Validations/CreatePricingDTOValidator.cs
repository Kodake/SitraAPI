using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreatePricingDTOValidator : AbstractValidator<CreatePricingDTO>
    {
        public CreatePricingDTOValidator(IUnitRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                _ = Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (name, _) =>
            {
                var exists = await repository.Exists(id, name);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.Name));

            RuleFor(x => x.PricingType)
            .IsInEnum().WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x.Percentage)
            .NotNull().WithMessage(x => string.Format(Utilities.RequiredFieldForConditionMessage, nameof(x.PricingType), nameof(x.Percentage)))
            .GreaterThan(0).WithMessage(Utilities.GreatherThanZero)
            .When(x => x.PricingType == PricingType.Percentage);
        }
    }
}


