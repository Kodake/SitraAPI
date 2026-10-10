using FluentValidation;
using SITRA.Core.DTOs.Products;

namespace SITRA.Core.Validations.Products
{
    public class UploadPictureDTOValidator : AbstractValidator<UploadPictureDTO>
    {
        public UploadPictureDTOValidator()
        {
            RuleFor(x => x.GetFile())
                .ValidImage(required: true)
                .WithName("File");
        }
    }
}
