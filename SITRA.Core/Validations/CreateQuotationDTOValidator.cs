using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateQuotationDTOValidator : AbstractValidator<CreateQuotationDTO>
    {
        public CreateQuotationDTOValidator(
            IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            RuleFor(x => x.BillerId)
                .MustAsync(async (id, _) => await unitOfWork.ContactRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.BillerId));

            RuleFor(x => x.CustomerId)
                .MustAsync(async (id, _) => await unitOfWork.ContactRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CustomerId));

            RuleFor(x => x.QuotationNumber)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await unitOfWork.CurrencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));
        }
    }
}

