using BackEnd.DTOs.Products;
using BackEnd.DTOs.Warehouses;
using BackEnd.Helpers;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Products;
using FluentValidation;

namespace BackEnd.Validations
    {
        public class CreateWarehouseDTOValidator : AbstractValidator<CreateWarehouseDTO>
        {
            public CreateWarehouseDTOValidator(IWarehouseRepository repository,
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
                }).WithMessage(x => $"Ya existe un almacén con el nombre {x.Name}");

                RuleFor(x => x.Address)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage)
                .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);

                RuleFor(x => x.Country)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);
        }
    }
}
