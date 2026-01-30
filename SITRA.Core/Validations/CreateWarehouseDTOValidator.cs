using FluentValidation;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Warehouses;

namespace SITRA.Core.Validations
{
    public class CreateWarehouseDTOValidator : AbstractValidator<CreateWarehouseDTO>
    {
        public CreateWarehouseDTOValidator(IWarehouseRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
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

            RuleFor(x => x.Address)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);
        }
    }
}


