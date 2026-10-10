using FluentValidation;
using SITRA.Core.DTOs.Wizard;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Finances;
using SITRA.Core.Interfaces.Warehouses;
using SITRA.Core.Validations.AdministrativeTools;
using SITRA.Core.Validations.Warehouses;

namespace SITRA.Core.Validations.Wizard
{
    public class WizardDTOValidator : AbstractValidator<WizardDTO>
    {
        public WizardDTOValidator(
            IUnitOfWork unitOfWork,
            ICurrencyRepository currencyRepository,
            IWarehouseRepository warehouseRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            RuleFor(x => x.Currency).SetValidator(new CreateCurrencyDTOValidator(currencyRepository, httpContextAccessor));
            RuleFor(x => x.Tax).SetValidator(new CreateTaxDTOValidator(httpContextAccessor));
            RuleFor(x => x.Product).SetValidator(new WizardProductDTOValidator());
            RuleFor(x => x.Warehouse).SetValidator(new CreateWarehouseDTOValidator(warehouseRepository, httpContextAccessor));
            RuleFor(x => x.Stock).NotNull();
            RuleFor(x => x.Stock.Qty).GreaterThanOrEqualTo(0);
        }
    }

    public class WizardProductDTOValidator : AbstractValidator<WizardProductDTO>
    {
        public WizardProductDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(500).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.InitialCost)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.BasePrice)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);
        }
    }
}
