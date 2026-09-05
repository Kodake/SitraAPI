using FluentValidation;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations.CashBoxes
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
                .WithMessage(Utilities.UserNotAuthMessage)
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
                    return string.Format(Utilities.PettyCashOwnedByMessage, otherOpenPettyCash?.User?.Email);
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
                .WithMessage(Utilities.AlreadyOpenPettyCashMessage)
                .WithName("UserPettyCash");
        }
    }
}

