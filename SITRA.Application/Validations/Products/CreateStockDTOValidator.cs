using FluentValidation;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Products
{
    public class CreateStockDTOValidator : AbstractValidator<CreateStockDTO>
    {
        public CreateStockDTOValidator(IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                _ = Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.Qty)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);
        }
    }
}


