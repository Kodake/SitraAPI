using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace SITRA.Core.Validations
{
    public class CreateSaleInvoiceDTOValidator : AbstractValidator<CreateSaleInvoiceDTO>
    {
        public CreateSaleInvoiceDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            // User authentication validation
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    return user != null;
                })
                .WithMessage("No user authenticated found.")
                .WithName("User");

            // Petty cash existence validation
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    return pettyCash != null;
                })
                .WithMessage("You need an opened petty cash to proceed.")
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
                    return $"Cannot create invoice. The petty cash is currently opened by {pettyCash?.User?.Email}. Please wait until it is closed.";
                })
                .WithName("PettyCashOwnership");

            // Currency existence validation
            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, cancellation) =>
                {
                    var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(id);
                    return currency != null;
                })
                .WithMessage((dto, id) => $"Currency with id {id} not found.");

            // Fiscal support existence validation
            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, cancellation) =>
                {
                    if (!id.HasValue) return true;

                    var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id.Value);
                    return fiscalSupport != null;
                })
                .WithMessage("Fiscal support not found.")
                .When(x => x.FiscalSupportId.HasValue);

            // Invoice items validation - each item must exist
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
                        .WithMessage(itemDTO => $"{itemDTO.ItemType} with id {itemDTO.ItemId} not found.");
                });

            // Stock existence validation for each item in the specified warehouse
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

                        if (item.ItemType == ItemType.Combo)
                        {
                            var combo = await unitOfWork.ComboRepository.GetByIdAsync(item.ItemId);
                            if (combo == null) continue; // Item existence check handles this

                            foreach (var comboItem in combo.ComboItems)
                            {
                                var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(comboItem.ChildItemId, dto.WarehouseId);
                                if (stock == null) return false;
                            }
                        }
                    }
                    return true;
                })
                .WithMessage("One or more items do not have stock configured in the specified warehouse.")
                .WithName("Stock");

            // Existing validations
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.PricingId));

            RuleFor(x => x.BillerId)
                .MustAsync(async (id, _) =>
                    !id.HasValue || await unitOfWork.ContactRepository.ExistsAsync(id.Value))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.BillerId));

            RuleFor(x => x.CustomerId)
                .MustAsync(async (id, _) =>
                    id.HasValue && await unitOfWork.ContactRepository.ExistsAsync(id.Value))
                .WithMessage(x => string.Format(Utilities.NotExistsWith, x.CustomerId));

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

            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
                    if (routeId != null && int.TryParse(routeId.ToString(), out int id) && (httpContextAccessor.HttpContext.Request.Path.Value?.Contains("process") == true))
                    {
                        var quotation = await unitOfWork.QuotationRepository.GetByIdAsync(id);
                        return quotation != null;
                    }
                    return true;
                })
                .WithMessage("Quotation not found.")
                .WithName("QuotationId");
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

