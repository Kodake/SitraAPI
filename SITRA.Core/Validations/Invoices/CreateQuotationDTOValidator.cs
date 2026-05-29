using FluentValidation;
using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Invoices
{
    public class CreateQuotationDTOValidator : AbstractValidator<CreateQuotationDTO>
    {
        public CreateQuotationDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            RuleFor(x => x.BillerId)
                .MustAsync(async (billerId, _) => await unitOfWork.ContactRepository.ExistsAsync(billerId!.Value))
                .When(x => x.BillerId.HasValue && x.BillerId.Value != Guid.Empty)
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.BillerId));

            RuleFor(x => x.CustomerId)
                .MustAsync(async (customerId, _) => await unitOfWork.ContactRepository.ExistsAsync(customerId!.Value))
                .When(x => x.CustomerId.HasValue && x.CustomerId.Value != Guid.Empty)
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CustomerId));

            RuleFor(x => x.QuotationNumber)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MustAsync(async (quotationNumber, _) =>
                {
                    var exists = await unitOfWork.QuotationRepository.ExistsAsync(id, quotationNumber);
                    return !exists;
                }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.QuotationNumber));

            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, _) => await unitOfWork.CurrencyRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CurrencyId));

            // Quotation items validation - each item must exist
            RuleForEach(x => x.InvoiceItems)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x)
                        .MustAsync(async (itemDTO, cancellation) =>
                        {
                            switch (itemDTO.ItemType)
                            {
                                case ItemType.Product:
                                    var product = await unitOfWork.ProductRepository.GetByIdAsync(itemDTO.ItemId);
                                    return product != null;
                                case ItemType.Service:
                                    var service = await unitOfWork.ServiceRepository.GetByIdAsync(itemDTO.ItemId);
                                    return service != null;
                                case ItemType.Combo:
                                    var combo = await unitOfWork.ComboRepository.GetByIdAsync(itemDTO.ItemId);
                                    return combo != null;
                                default:
                                    return false;
                            }
                        })
                        .WithMessage(itemDTO => string.Format(Utilities.ItemNotFoundMessage, itemDTO.ItemType, itemDTO.ItemId));
                });
        }
    }
}

