using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Mediator.Commands.Tenants
{
    public record CompleteSubUserRegistrationCommand(CompleteInvitationDTO DTO) : IRequest<IdentityResult>;

    public class CompleteSubUserRegistrationHandler : IRequestHandler<CompleteSubUserRegistrationCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public CompleteSubUserRegistrationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(CompleteSubUserRegistrationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.InvitationToken == request.DTO.Token, cancellationToken);

            if (user == null || user.InvitationExpiry < DateTimeOffset.UtcNow)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired invitation token." });
            }

            // Update user details
            user.UserName = user.Email; // Use invited email as username
            user.InvitationToken = null;
            user.InvitationExpiry = null;

            var result = await _userManager.AddPasswordAsync(user, request.DTO.Password);
            if (!result.Succeeded) return result;

            return await _userManager.UpdateAsync(user);
        }
    }
}
