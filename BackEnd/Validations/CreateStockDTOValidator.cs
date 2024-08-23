using BackEnd.DTOs.Warehouses;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateStockDTOValidator : AbstractValidator<CreateStockDTO>
    {
        public CreateStockDTOValidator(IProductRepository productRepository,
                                       IWarehouseRepository warehouseRepository,
                                       IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.WarehouseId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.Qty)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage("La cantidad debe ser un valor positivo.");
        }
    }
}
