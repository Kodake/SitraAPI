using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Helpers;
using BackEnd.Repositories;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateSaleInvoiceDTOValidator : AbstractValidator<CreateSaleInvoiceDTO>
    {
        public CreateSaleInvoiceDTOValidator(IUnitOfWork unitOfWork)
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

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) => await unitOfWork.FiscalSupportRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.FiscalSupportId));

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await unitOfWork.CurrencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                {
                    var exists = await unitOfWork.FiscalSupportRepository.ExistsAsync(id);
                    if (!exists) return false;

                    var support = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id);
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