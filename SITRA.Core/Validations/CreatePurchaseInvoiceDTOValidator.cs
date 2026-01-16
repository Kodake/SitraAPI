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

            // Purchase invoice items validation
            RuleForEach(x => x.InvoiceItems)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ItemId)
                        .MustAsync(async (itemId, cancellation) =>
                        {
                            var product = await unitOfWork.ProductRepository.GetByIdAsync(itemId);
                            return product != null;
                        })
                        .WithMessage("Product with id {PropertyValue} not found.");
                });

            // Stock existence validation for each product in the specified warehouse
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    foreach (var item in dto.InvoiceItems)
                    {
                        if (item.ItemType == SITRA.Core.Enums.ItemType.Product)
                        {
                            var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);
                            if (stock == null) return false;
                        }
                    }
                    return true;
                })
                .WithMessage("One or more items do not have stock configured in the specified warehouse.")
                .WithName("Stock");
        }
    }
}

