using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.Enums;
using BackEnd.Helpers;
using BackEnd.Repositories;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateReceivableDTOValidator : AbstractValidator<CreatePaymentReceiptDTO>
    {
        public CreateReceivableDTOValidator(IUnitOfWork repository,
          IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.PaymentMethod)
                .Must(value => Enum.IsDefined(typeof(PaymentMethod), value))
                .WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x.TransactionType)
                .Must(value => Enum.IsDefined(typeof(TransactionType), value))
                .WithMessage(Utilities.InvalidEnumValueMessage);
        }
    }
}