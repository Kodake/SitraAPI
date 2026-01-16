using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateOpenPettyCashDTOValidator : AbstractValidator<CreateOpenPettyCashDTO>
    {
        public CreateOpenPettyCashDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.OpeningAmount)
                .GreaterThanOrEqualTo(0).WithMessage(Utilities.GreaterThanOrEqualToZero);

            // Validate user is authenticated
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    return user != null;
                })
                .WithMessage("No user authenticated found.")
                .WithName("User");

            // Validate no other user has petty cash open
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var otherOpenPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();
                    if (otherOpenPettyCash == null) return true;

                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user != null && otherOpenPettyCash.UserId == user.Id) return true;

                    return false;
                })
                .WithMessage((dto, context) =>
                {
                    var otherOpenPettyCash = unitOfWork.PettyCashRepository.GetOpenPettyCashAsync().Result;
                    return $"Petty cash is currently opened by {otherOpenPettyCash?.User?.Email}. Please wait until it is closed.";
                })
                .WithName("PettyCash");

            // Validate current user doesn't already have an open petty cash
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true; // Skip if user validation already failed

                    var existingUserPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
                    return existingUserPettyCash == null;
                })
                .WithMessage("You already have an open petty cash. Please close it before opening a new one.")
                .WithName("UserPettyCash");
        }
    }
}

