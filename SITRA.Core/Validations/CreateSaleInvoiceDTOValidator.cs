using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateSaleInvoiceDTOValidator : AbstractValidator<CreateSaleInvoiceDTO>
    {
        public CreateSaleInvoiceDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            RuleFor(x => x.BillerId)
                .MustAsync(async (id, _) =>
                    !id.HasValue || await unitOfWork.ContactRepository.ExistsAsync(id.Value))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.BillerId));


            RuleFor(x => x.BillerId)
                .MustAsync(async (id, _) =>
                    !id.HasValue || await unitOfWork.ContactRepository.ExistsAsync(id.Value))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CustomerId));

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                    !id.HasValue || await unitOfWork.FiscalSupportRepository.ExistsAsync(id.Value))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.FiscalSupportId));

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await unitOfWork.CurrencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                {
                    if (!id.HasValue)
                        return true;

                    var exists = await unitOfWork.FiscalSupportRepository.ExistsAsync(id.Value);
                    if (!exists) return false;

                    var support = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id.Value);
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

