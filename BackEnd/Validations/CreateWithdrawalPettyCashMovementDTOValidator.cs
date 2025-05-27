using BackEnd.DTOs.CashBoxes;
using BackEnd.Helpers;
using BackEnd.Repositories;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateWithdrawalPettyCashMovementDTOValidator : AbstractValidator<CreateWithdrawalPettyCashMovementDTO>
    {
        public CreateWithdrawalPettyCashMovementDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.WithdrawalAmount)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero)
                .MustAsync(async (dto, amount, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null)
                        return false;

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
                    if (pettyCash == null)
                        return false;

                    return amount <= pettyCash.ClosingAmount;
                })
                .WithMessage(Utilities.WithdrawnAmountWontBeGreatherThanAvailableCash);
        }
    }
}
