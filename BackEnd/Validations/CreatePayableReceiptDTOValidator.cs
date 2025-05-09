using BackEnd.DTOs.Accounts.PayableReceipts;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories.Accounts;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreatePayableReceiptDTOValidator : AbstractValidator<CreatePayableReceiptDTO>
    {
        public CreatePayableReceiptDTOValidator(IPayableRepository payableReceiptRepository,
                                                IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                _ = int.TryParse(valorString, out id);
            }

            RuleFor(dto => id)
                .MustAsync(async (id, _) => await payableReceiptRepository.ExistsAsync(id))
                .WithMessage(Utilities.IdNotExistMessage)
                .DependentRules(() =>
                {
                    RuleFor(_ => id)
                        .MustAsync(async (payableId, _) =>
                        {
                            var payable = await payableReceiptRepository.GetByIdAsync(payableId);
                            return payable == null || payable.Status != AccountStatus.Paid;
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
