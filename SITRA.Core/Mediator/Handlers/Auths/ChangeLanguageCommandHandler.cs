using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Mediator.Commands.Auths;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class ChangeLanguageCommandHandler : IRequestHandler<ChangeLanguageCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ChangeLanguageCommandHandler> _logger;

        public ChangeLanguageCommandHandler(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChangeLanguageCommandHandler> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> Handle(ChangeLanguageCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("ChangeLanguage attempt without valid UserId in context.");
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                return false;
            }

            user.Language = request.Dto.Language;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to update language for user {UserId}: {Errors}", userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }

            return true;
        }
    }
}
