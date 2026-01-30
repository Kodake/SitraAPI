using FluentValidation;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations
{
    public class CreateReceivableDTOValidator : AbstractValidator<CreatePaymentReceiptDTO>
    {
        public CreateReceivableDTOValidator(IUnitOfWork repository,
          IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.PaymentMethod)
                .Must(value => Enum.IsDefined(typeof(PaymentMethod), value))
                .WithMessage(Utilities.InvalidEnumValueMessage);
        }
    }
}


