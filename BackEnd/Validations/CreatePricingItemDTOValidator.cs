using BackEnd.DTOs.Products;
using BackEnd.DTOs.Warehouses;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreatePricingItemDTOValidator : AbstractValidator<CreatePricingItemDTO>
    {
        public CreatePricingItemDTOValidator(IPricingItemRepository repository,
                                       IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.PricingId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.InitialCost)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage("The initial cost must be a positive value.");

            RuleFor(x => x.BasePrice)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage("The base price must be a positive value.");


            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                    !await repository.Exists(dto.PricingId, dto.ItemId))
                .WithMessage("A PricingItem with the same PricingId and ItemId already exists.");
        }
    }
}
