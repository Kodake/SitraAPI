using BackEnd.DTOs;
using BackEnd.Repositories;
using BackEnd.Helpers;
using FluentValidation;
using BackEnd.DTOs.Products;
using BackEnd.Repositories.Products;

namespace BackEnd.Validations
{
    public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
    {
        public CreateCategoryDTOValidator(ICategoryRepository repository,
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
            .MinimumLength(3).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (nombre, _) =>
            {
                var existe = await repository.Exists(id, nombre);
                return !existe;
            }).WithMessage(x => $"Ya existe una categoría con el nombre {x.Name}");

            RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}
