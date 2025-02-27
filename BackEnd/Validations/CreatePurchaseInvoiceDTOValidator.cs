using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.Helpers;
using BackEnd.Repositories;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreatePurchaseInvoiceDTOValidator : AbstractValidator<CreatePurchaseInvoiceDTO>
    {
        private IUnitOfWork unitOfWork;

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
