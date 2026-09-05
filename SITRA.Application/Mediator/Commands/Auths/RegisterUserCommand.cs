using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class RegisterUserCommand : AuthCommand<IdentityResult>, IIdempotentCommand
    {
        public CreateRegistrationUserDTO Dto { get; }

        public RegisterUserCommand(CreateRegistrationUserDTO dto)
        {
            Dto = dto;
        }

        public override async Task<IdentityResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var result = await unitOfWork.UserRegistrationService.RegisterAsync(Dto, cancellationToken);

            if (result.IdentityResult.Succeeded && result.UserId != null && result.AdminRoleId.HasValue)
            {
                await unitOfWork.PermissionClaimService.AddPermissionClaimsFromRolesAsync(
                    result.UserId, new[] { result.AdminRoleId.Value }, cancellationToken);
            }

            return result.IdentityResult;
        }
    }
}
