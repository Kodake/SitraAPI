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

        public ChangeLanguageCommandHandler(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(ChangeLanguageCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Language = request.Dto.Language;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
