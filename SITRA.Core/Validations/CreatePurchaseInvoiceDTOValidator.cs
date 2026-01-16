using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreatePurchaseInvoiceDTOValidator : AbstractValidator<CreatePurchaseInvoiceDTO>
    {
        private readonly IUnitOfWork unitOfWork;
        public CreatePurchaseInvoiceDTOValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            RuleFor(x => x.SupplierId)
                .MustAsync(async (id, _) => await unitOfWork.ContactRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.SupplierId));

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await unitOfWork.CurrencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));
        }
    }
}

