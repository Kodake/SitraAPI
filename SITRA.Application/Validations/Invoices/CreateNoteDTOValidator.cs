using FluentValidation;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Tenants;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Invoices
{
    public class CreateNoteDTOValidator : AbstractValidator<CreateNoteDTO>
    {
        public CreateNoteDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
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

            // Plan Limit Validation (Moved from Command)
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user != null)
                    {
                        var tenant = await unitOfWork.Repository<Tenant>().GetByIdAsync(user.TenantId);
                        var monthlySalesLimit = unitOfWork.PlanService.GetMaxMonthlySales(tenant!.Plan);

                        if (monthlySalesLimit.HasValue)
                        {
                            var currentMonth = DateTime.UtcNow.Month;
                            var currentYear = DateTime.UtcNow.Year;

                            var salesCount = await unitOfWork.SaleInvoiceRepository
                                .CountAsync(x => x.Date.Month == currentMonth && x.Date.Year == currentYear);

                            if (salesCount >= monthlySalesLimit.Value)
                            {
                                return false; // Validation failed
                            }
                        }
                    }
                    return true;
                })
                .WithMessage((dto) =>
                {
                    var user = unitOfWork.UserService.GetUserAsync().Result;
                    if (user != null)
                    {
                        var tenant = unitOfWork.Repository<Tenant>().GetByIdAsync(user.TenantId).Result;
                        if (tenant != null)
                        {
                            var monthlySalesLimit = unitOfWork.PlanService.GetMaxMonthlySales(tenant.Plan);
                            return Utilities.MonthlySalesLimitReachedMessage(monthlySalesLimit ?? 0, tenant.Plan.ToString());
                        }
                    }
                    return Utilities.MonthlySalesLimitReachedMessage(0, string.Empty);
                })
                .WithName("PlanLimit");


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

            // Currency existence validation
            RuleFor(x => x.CurrencyId)
                .MustAsync(async (id, cancellation) =>
                {
                    var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(id);
                    return currency != null;
                })
                .WithMessage((dto, id) => string.Format(Utilities.CurrencyNotFoundMessage, id));

            // Fiscal support existence validation
            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, cancellation) =>
                {
                    if (!id.HasValue) return true;

                    var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id.Value);
                    return fiscalSupport != null;
                })
                .WithMessage(Utilities.FiscalSupportNotFoundMessage)
                .When(x => x.FiscalSupportId.HasValue);

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                {
                    if (!id.HasValue) return true;

                    var fiscalSupport = await unitOfWork
                        .FiscalSupportRepository
                        .GetByIdAsync(id.Value);

                    return fiscalSupport != null
                        && fiscalSupport.InitialSequence < fiscalSupport.FinalSequence;
                })
                .WithMessage(dto =>
                {
                    var fiscalSupport = unitOfWork
                        .FiscalSupportRepository
                        .GetByIdAsync(dto.FiscalSupportId!.Value)
                        .Result;

                    return Utilities.FiscalSupportSequenceLimitReachedMessage(
                        fiscalSupport!.FinalSequence
                    );
                });

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
                        .WithMessage(itemDTO => string.Format(Utilities.ItemNotFoundMessage, itemDTO.ItemType, itemDTO.ItemId));
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
                .WithMessage(Utilities.StockNotConfiguredMessage)
                .WithName("Stock");

            // Existing validations
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(Utilities.NotExistsWith);

            RuleFor(x => x.CustomerId)
                .MustAsync(async (id, _) =>
                    !id.HasValue || await unitOfWork.ContactRepository.ExistsAsync(id.Value))
                .WithMessage(Utilities.NotExistsWith)
                .When(x => x.CustomerId.HasValue && x.CustomerId.Value != Guid.Empty);

            RuleFor(x => x.FiscalSupportId)
                .MustAsync(async (id, _) =>
                {
                    if (!id.HasValue) return true;

                    var fiscalSupport = await unitOfWork
                        .FiscalSupportRepository
                        .GetByIdAsync(id.Value);

                    return fiscalSupport != null
                        && fiscalSupport.InitialSequence < fiscalSupport.FinalSequence;
                })
                .WithMessage(dto =>
                {
                    var fiscalSupport = unitOfWork
                        .FiscalSupportRepository
                        .GetByIdAsync(dto.FiscalSupportId!.Value)
                        .Result;

                    return Utilities.FiscalSupportSequenceLimitReachedMessage(
                        fiscalSupport!.FinalSequence
                    );
                });

            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
                    if (routeId != null && Guid.TryParse(routeId.ToString(), out Guid id) && (httpContextAccessor.HttpContext.Request.Path.Value?.Contains("process") == true))
                    {
                        var quotation = await unitOfWork.QuotationRepository.GetByIdAsync(id);
                        return quotation != null;
                    }
                    return true;
                })
                .WithMessage(Utilities.QuotationNotFoundMessage)
                .WithName("QuotationId");
        }
    }
}

