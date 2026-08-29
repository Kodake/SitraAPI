using FluentValidation;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Tenants;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.Invoices
{
    public class CreateSaleInvoiceWithItemDTOValidator : AbstractValidator<CreateSaleInvoiceWithItemDTO>
    {
        public CreateSaleInvoiceWithItemDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            // ── Item-specific validations ──

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

            // ── Standard Sale Invoice validations (same as CreateSaleInvoiceDTOValidator) ──

            // User authentication validation
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    return user != null;
                })
                .WithMessage(Utilities.UserNotAuthMessage)
                .WithName("User");

            // Plan Limit Validation
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
                                return false;
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
                    if (user == null) return true;

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    if (pettyCash == null) return true;

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

            // Existing validations
            RuleFor(x => x.PricingId)
                .MustAsync(async (id, _) => await unitOfWork.PricingRepository.ExistsAsync(id))
                .WithMessage(Utilities.NotExistsWith);

            RuleFor(x => x.BillerId)
                .MustAsync(async (id, _) => await unitOfWork.ContactRepository.ExistsAsync(id!.Value))
                .WithMessage(Utilities.NotExistsWith)
                .When(x => x.BillerId.HasValue && x.BillerId.Value != Guid.Empty);

            RuleFor(x => x.CustomerId)
                .MustAsync(async (id, _) => await unitOfWork.ContactRepository.ExistsAsync(id!.Value))
                .WithMessage(Utilities.NotExistsWith)
                .When(x => x.CustomerId.HasValue && x.CustomerId.Value != Guid.Empty);

            // Note: InvoiceItems validation is skipped here because the item hasn't been created yet.
            // The Command itself handles item creation and injection into InvoiceItems.
        }
    }
}
