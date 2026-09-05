using FluentValidation;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.CashBoxes
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
                .WithMessage(Utilities.UserNotAuthMessage)
                .WithName("User");

            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return true;

                    var pettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashByUserIdAsync(user.Id);
                    return pettyCash != null;
                })
                .WithMessage(Utilities.NoOpenPettyCashMessage)
                .WithName("PettyCash");

            RuleFor(x => x.Comments)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);
        }
    }
}

