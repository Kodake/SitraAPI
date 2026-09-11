using FluentValidation;
using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Invoices
{
    public class CreateQuotationWithItemDTOValidator : AbstractValidator<CreateQuotationWithItemDTO>
    {
        public CreateQuotationWithItemDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var quotationGuid = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out quotationGuid);
            }

            // ── Item-specific validations ─┃
            RuleFor(x => x.ItemType)
                .Must(type => type == ItemType.Product || type == ItemType.Service)
                .WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x.ItemName)
                .NotEmpty()
                .WithMessage(Utilities.RequiredFieldMessage)
                .When(x => x.InvoiceItems == null || x.InvoiceItems.Count == 0);

            RuleFor(x => x.ItemBasePrice)
                .GreaterThan(0)
                .WithMessage(Utilities.GreatherThanZero)
                .When(x => x.InvoiceItems == null || x.InvoiceItems.Count == 0);

            RuleForEach(x => x.TaxIds)
                .MustAsync(async (taxId, cancellation) =>
                {
                    var tax = await unitOfWork.TaxRepository.GetByIdAsync(taxId);
                    return tax != null;
                })
                .WithMessage(Utilities.TaxNotFoundMessage);

            // ── Standard Quotation validations ─┃
            RuleFor(x => x.PricingId)
                .MustAsync(async (pricingId, _) => await unitOfWork.PricingRepository.ExistsAsync(pricingId))
                .When(x => x.PricingId != Guid.Empty)
                .WithMessage(Utilities.NotExistsWith);

            RuleFor(x => x.WarehouseId)
                .MustAsync(async (warehouseId, _) => await unitOfWork.WarehouseRepository.ExistsAsync(warehouseId))
                .When(x => x.WarehouseId != Guid.Empty)
                .WithMessage(Utilities.NotExistsWith);

            RuleFor(x => x.BillerId)
                .MustAsync(async (billerId, _) => await unitOfWork.ContactRepository.ExistsAsync(billerId!.Value))
                .When(x => x.BillerId.HasValue && x.BillerId.Value != Guid.Empty)
                .WithMessage(Utilities.NotExistsWith);

            RuleFor(x => x.CustomerId)
                .MustAsync(async (customerId, _) => await unitOfWork.ContactRepository.ExistsAsync(customerId!.Value))
                .When(x => x.CustomerId.HasValue && x.CustomerId.Value != Guid.Empty)
                .WithMessage(Utilities.NotExistsWith);

            RuleFor(x => x.QuotationNumber)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MustAsync(async (quotationNumber, _) =>
                {
                    var exists = await unitOfWork.QuotationRepository.ExistsAsync(quotationGuid, quotationNumber);
                    return !exists;
                }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.QuotationNumber));

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (currencyId, _) => await unitOfWork.CurrencyRepository.ExistsAsync(currencyId))
                .WithMessage(Utilities.NotExistsWith);
        }
    }
}
