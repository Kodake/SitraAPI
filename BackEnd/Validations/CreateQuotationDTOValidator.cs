using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Helpers;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Products;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateQuotationDTOValidator : AbstractValidator<CreateQuotationDTO>
    {
        public CreateQuotationDTOValidator(
            IPricingRepository pricingRepository,
            IContactRepository contactRepository,
            ICurrencyRepository currencyRepository)
        {
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await pricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            RuleFor(x => x.BillerId)
                .MustAsync(async (id, _) => await contactRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.BillerId));

            RuleFor(x => x.CustomerId)
                .MustAsync(async (id, _) => await contactRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CustomerId));

            RuleFor(x => x.QuotationNumber)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await currencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));
        }
    }
}