using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateClosePettyCashDTOValidator : AbstractValidator<CreateClosePettyCashDTO>
    {
        public CreateClosePettyCashDTOValidator(SITRA.Core.Interfaces.IUnitOfWork unitOfWork)
        {
            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    return user != null;
                })
                .WithMessage("No authenticated user found.")
                .WithName("User");

            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true;

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
                    return pettyCash != null;
                })
                .WithMessage("You don't have an open petty cash.")
                .WithName("PettyCash");

            RuleFor(x => x.Comments)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}

