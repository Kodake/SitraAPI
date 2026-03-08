using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using AutoMapper;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class CreateAssociatedUserCommand : AuthCommand<IdentityResult>
    {
        public CreateAssociatedUserDTO Dto { get; }

        public CreateAssociatedUserCommand(CreateAssociatedUserDTO dto)
        {
            Dto = dto;
        }

        public override async Task<IdentityResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var result = await unitOfWork.UserRegistrationService.RegisterAssociatedUserAsync(Dto, cancellationToken);

            if (result.IdentityResult.Succeeded && result.UserId != null && result.AdminRoleId.HasValue)
            {
                await unitOfWork.PermissionClaimService.AddPermissionClaimsFromRolesAsync(
                    result.UserId, new[] { result.AdminRoleId.Value }, cancellationToken);
            }

            return result.IdentityResult;
        }
    }
}
