using FluentValidation;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Products;

namespace SITRA.Core.Validations.Products
{
    public class CreateBrandDTOValidator : AbstractValidator<CreateBrandDTO>
    {
        public CreateBrandDTOValidator(IUnitOfWork repository,
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
                var exists = await repository.BrandRepository.Exists(id, name);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.Name));
        }
    }
}


