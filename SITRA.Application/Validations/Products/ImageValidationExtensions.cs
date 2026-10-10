using FluentValidation;
using Microsoft.AspNetCore.Http;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Products
{
    public static class ImageValidationExtensions
    {
        public const long MaxImageSizeBytes = 300 * 1024; // 300 KB

        public static IRuleBuilderOptions<T, IFormFile?> ValidImage<T>(this IRuleBuilder<T, IFormFile?> ruleBuilder, bool required = false)
        {
            if (required)
            {
                ruleBuilder
                    .NotNull().WithMessage(Utilities.RequiredFieldMessage)
                    .Must(f => f != null && f.Length > 0).WithMessage(Utilities.RequiredFieldMessage);
            }

            return ruleBuilder
                .Must(f => f == null || f.Length <= MaxImageSizeBytes)
                .WithMessage(Utilities.ImageMaxSizeMessage);
        }
    }
}
