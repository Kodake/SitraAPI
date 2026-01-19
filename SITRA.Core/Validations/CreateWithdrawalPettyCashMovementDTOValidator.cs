using SITRA.Core.Entities.Auths;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateWithdrawalPettyCashMovementDTOValidator : AbstractValidator<CreateWithdrawalPettyCashMovementDTO>
    {
        public CreateWithdrawalPettyCashMovementDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.WithdrawalAmount)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .GreaterThan(0).WithMessage(Utilities.GreatherThanZero);

            // Validate user is authenticated
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    return user != null;
                })
                .WithMessage(Utilities.UserNotAuthMessage)
                .WithName("User");

            // Validate no other user has petty cash open
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true; // Skip if user validation already failed

                    var otherOpenPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    return otherOpenPettyCash == null || otherOpenPettyCash.UserId == user.Id;
                })
                .MustAsync(async (dto, cancellation) =>
                {
                    // This is used to get the dynamic error message with the user's email
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true;

                    var otherOpenPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    if (otherOpenPettyCash == null || otherOpenPettyCash.UserId == user.Id)
                        return true;

                    // Store the email in context for the error message
                    return false;
                })
                .WithMessage((dto, context) =>
                {
                    // Get the email from the repository for the error message
                    var otherOpenPettyCash = unitOfWork.PettyCashRepository.GetOpenPettyCashAsync().Result;
                    return string.Format(Utilities.PettyCashOwnedByMessage, otherOpenPettyCash?.User?.Email);
                })
                .WithName("PettyCash");

            // Validate current user has an open petty cash
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true; // Skip if user validation already failed

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
                    return pettyCash != null;
                })
                .WithMessage(Utilities.NoOpenPettyCashMessage)
                .WithName("UserPettyCash");

            // Validate withdrawal amount doesn't exceed available cash
            RuleFor(x => x.WithdrawalAmount)
                .MustAsync(async (dto, amount, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true; // Skip if user validation already failed

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
                    if (pettyCash == null) return true; // Skip if petty cash validation already failed

                    return amount <= pettyCash.ClosingAmount;
                })
                .WithMessage(Utilities.WithdrawnAmountWontBeGreatherThanAvailableCash);
        }
    }
}


