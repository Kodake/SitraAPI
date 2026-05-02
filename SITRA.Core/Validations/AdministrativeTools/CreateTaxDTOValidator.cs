using FluentValidation;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.AdministrativeTools
{
    public class CreateTaxDTOValidator : AbstractValidator<CreateTaxDTO>
    {
        public CreateTaxDTOValidator(IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.TaxType)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .Must(x => Enum.IsDefined(typeof(TaxType), x)).WithMessage(Utilities.InvalidTaxTypeMessage);


            //IF TaxType IS OTHER, AMOUNT COULD BE FROM 0 TO 100
            When(x => x.TaxType == TaxType.OTRO, () =>
            {
                RuleFor(x => x.Amount)
                .Must(Utilities.IsValidPercentage).WithMessage(Utilities.InvalidPercentageMessage)
                .Must(x => x >= 0 && x <= 100).WithMessage(Utilities.InvalidTaxAmountMessage);
            });

            When(x => x.TaxType == TaxType.ITBIS, () =>
            {
                RuleFor(x => x.Amount)
                .Must(Utilities.IsValidPercentage).WithMessage(Utilities.InvalidPercentageMessage)
                .When(x => x.Amount != 0)
                .Must(x => x == 0 || x == 16 || x == 18).WithMessage(Utilities.InvalidITBISRateMessage);
            });

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(20).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);
        }
    }
}


