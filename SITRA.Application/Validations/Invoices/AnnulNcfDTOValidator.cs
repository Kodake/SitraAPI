using FluentValidation;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Invoices
{
    public class AnnulNcfDTOValidator : AbstractValidator<AnnulNcfDTO>
    {
        public AnnulNcfDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .WithName("InvoiceId");

            RuleFor(x => x.FiscalSupportId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .WithName("FiscalSupportId");

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                {
                    if (id == Guid.Empty) return true;
                    return await unitOfWork.FiscalSupportRepository.ExistsAsync(id);
                })
                .WithMessage(Utilities.FiscalSupportNotFoundMessage)
                .WithName("FiscalSupportId");
        }
    }
}
