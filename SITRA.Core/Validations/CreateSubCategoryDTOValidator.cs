using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateSubCategoryDTOValidator : AbstractValidator<CreateSubCategoryDTO>
    {
        public CreateSubCategoryDTOValidator(IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                _ = int.TryParse(valorString, out id);
            }

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(3).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (name, _) =>
            {
                var exists = await unitOfWork.SubCategoryRepository.Exists(id, name);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.Name));

            RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}


