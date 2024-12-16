using BackEnd.DTOs.Invoices;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Helpers;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Products;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateSaleInvoiceDTOValidator : AbstractValidator<CreateSaleInvoiceDTO>
    {
        public CreateSaleInvoiceDTOValidator(
            IPricingRepository pricingRepository,
            IContactRepository contactRepository,
            IFiscalSupportRepository fiscalSupportRepository,
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

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) => await fiscalSupportRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.FiscalSupportId));

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await currencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                {
                    var exists = await fiscalSupportRepository.ExistsAsync(id);
                    if (!exists) return false;

                    var support = await fiscalSupportRepository.GetByIdAsync(id);
                    return TryUpdateFiscalSupportSequence(support!);
                })
                .WithMessage(Utilities.MaximumSupportsReachedMessage);
        }

        private static bool TryUpdateFiscalSupportSequence(FiscalSupport fiscalSupport)
        {
            const int increment = 1;
            var nextSequence = fiscalSupport.InitialSequence + increment;
            if (nextSequence > fiscalSupport.FinalSequence) return false;

            fiscalSupport.InitialSequence = nextSequence;
            return true;
        }
    }
}