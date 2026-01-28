using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreatePayableDTOValidator : AbstractValidator<CreatePayableReceiptDTO>
    {
        public CreatePayableDTOValidator(IUnitOfWork repository,
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


