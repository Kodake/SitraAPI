using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreatePricingItemDTOValidator : AbstractValidator<CreatePricingItemDTO>
    {
        public CreatePricingItemDTOValidator(IPricingItemRepository repository,
                                       IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.PricingId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.InitialCost)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.BasePrice)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);


            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                    !await repository.ExistsAsync(dto.PricingId, dto.ItemId))
            .WithMessage(dto => string.Format(Utilities.NameAlreadyExistsWith, $"PricingId: {dto.PricingId} & ItemId: {dto.ItemId}"));
        }
    }
}


