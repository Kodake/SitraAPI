using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using System.Security.Claims;

namespace SITRA.Core.Validations.Auths
{
    public class ChangeLanguageDTOValidator : AbstractValidator<ChangeLanguageDTO>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public ChangeLanguageDTOValidator(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

            RuleFor(x => x.Language)
                .IsInEnum().WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x).CustomAsync(async (dto, context, cancellationToken) =>
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    context.AddFailure("User", Utilities.UserNotAuthMessage);
                    return;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    context.AddFailure("User", Utilities.UserNotAuthMessage);
                }
            });
        }
    }
}
