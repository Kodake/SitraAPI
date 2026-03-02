using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.Auths
{
    public class CreateLoginUserDTOValidator : AbstractValidator<CreateLoginUserDTO>
    {
        private readonly UserManager<User> _userManager;

        public CreateLoginUserDTOValidator(UserManager<User> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .EmailAddress().WithMessage(Utilities.EmailMessage);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x)
                .MustAsync(async (dto, cancellationToken) =>
                {
                    var user = await _userManager.Users
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(u => u.Email == dto.Email);
                    if (user == null) return false;

                    var result = await _userManager.CheckPasswordAsync(user, dto.Password);
                    return result;
                })
                .WithMessage(Utilities.InvalidCredentialsMessage);
        }
    }
}
