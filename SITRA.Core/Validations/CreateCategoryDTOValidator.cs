using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using FluentValidation;

namespace SITRA.Core.Validations
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
                _ = int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(3).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (name, _) =>
            {
                var exists = await repository.Exists(id, name);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.Name));

            RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}


