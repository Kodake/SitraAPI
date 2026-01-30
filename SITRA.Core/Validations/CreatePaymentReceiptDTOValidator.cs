using FluentValidation;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Accounts;

namespace SITRA.Core.Validations
{
    public class CreatePaymentReceiptDTOValidator : AbstractValidator<CreatePaymentReceiptDTO>
    {
        public CreatePaymentReceiptDTOValidator(IReceivableRepository receivableRepository,
                                                IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                _ = Guid.TryParse(valorString, out id);
            }

            RuleFor(dto => id)
                .MustAsync(async (id, _) => await receivableRepository.ExistsAsync(id))
                .WithMessage(Utilities.IdNotExistMessage)
                .DependentRules(() =>
                {
                    RuleFor(_ => id)
                        .MustAsync(async (receivableId, _) =>
                        {
                            var receivable = await receivableRepository.GetByIdAsync(receivableId);
                            return receivable == null || receivable.Status != AccountStatus.Paid;
                        })
                        .WithMessage(Utilities.ReceiptAlreadyPaidMessage);
                });

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(x => Utilities.GreatherThanZero);

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage(Utilities.InvalidEnumValueMessage);
        }
    }
}


