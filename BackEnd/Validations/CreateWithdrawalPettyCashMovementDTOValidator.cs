using BackEnd.DTOs.CashBoxes;
using BackEnd.Helpers;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateWithdrawalPettyCashMovementDTOValidator : AbstractValidator<CreateWithdrawalPettyCashMovementDTO>
    {
        public CreateWithdrawalPettyCashMovementDTOValidator()
        {
            RuleFor(x => x.WithdrawalAmount)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);
        }
    }
}
