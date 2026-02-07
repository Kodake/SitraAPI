using FluentValidation;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Invoices
{
    public class CreatePurchaseInvoiceDTOValidator : AbstractValidator<CreatePurchaseInvoiceDTO>
    {
        public CreatePurchaseInvoiceDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            // User authentication validation
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    return user != null;
                })
                .WithMessage(Utilities.UserNotAuthMessage)
                .WithName("User");

            // Petty cash existence validation
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    return pettyCash != null;
                })
                .WithMessage(Utilities.PettyCashRequiredMessage)
                .WithName("PettyCash");

            // Petty cash ownership validation
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true; // Skip if user validation already failed

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    if (pettyCash == null) return true; // Skip if petty cash validation already failed

                    return pettyCash.UserId == user.Id;
                })
                .WithMessage((dto, context) =>
                {
                    var pettyCash = unitOfWork.PettyCashRepository.GetOpenPettyCashAsync().Result;
                    return string.Format(Utilities.PettyCashOwnedByMessage, pettyCash?.User?.Email);
                })
                .WithName("PettyCashOwnership");

            // Pricing existence validation
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            // Supplier existence validation
            RuleFor(x => x.SupplierId)
                .MustAsync(async (id, _) => await unitOfWork.ContactRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.SupplierId));

            // Currency existence validation
            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, cancellation) =>
                {
                    var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(id);
                    return currency != null;
                })
                .WithMessage((dto, id) => string.Format(Utilities.CurrencyNotFoundMessage, id));

            // Purchase invoice items validation - each item must exist
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

            // Stock existence validation for each product in the specified warehouse
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    foreach (var item in dto.InvoiceItems)
                    {
                        if (item.ItemType == ItemType.Product)
                        {
                            var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(item.ItemId, dto.WarehouseId);
                            if (stock == null) return false;
                        }
                    }
                    return true;
                })
                .WithMessage(Utilities.StockNotConfiguredMessage)
                .WithName("Stock");
        }
    }
}

