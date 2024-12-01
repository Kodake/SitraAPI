using BackEnd.DTOs.Accounts;
using BackEnd.Helpers;
using BackEnd.Repositories.Accounts;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Products;
using BackEnd.Repositories.Warehouses;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreatePaymentReceiptDTOValidator : AbstractValidator<CreatePaymentReceiptDTO>
    {
        public CreatePaymentReceiptDTOValidator(
                                                IReceivableRepository receivableRepository,
                                                IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.ReceivableId)
                .MustAsync(async (receivableId, _) => await receivableRepository.Exists(receivableId))
                .WithMessage(x =>
                    string.Format(
                        "The ReceivableId '{0}' does not exist in the system. Please check and try again.",
                        x.ReceivableId
                    ));


            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x.TransactionType)
                .IsInEnum().WithMessage(Utilities.InvalidEnumValueMessage);
        }
    }
}