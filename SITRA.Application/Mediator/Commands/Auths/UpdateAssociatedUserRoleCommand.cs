using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using AutoMapper;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class UpdateAssociatedUserRoleCommand : AuthCommand<IdentityResult>
    {
        public string UserId { get; }
        public UpdateUserRoleDTO Dto { get; }

        public UpdateAssociatedUserRoleCommand(string userId, UpdateUserRoleDTO dto)
        {
            UserId = userId;
            Dto = dto;
        }

        public override async Task<IdentityResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var result = await unitOfWork.UserRoleService.UpdateUserRoleAsync(UserId, Dto, cancellationToken);

            if (result.Succeeded)
            {
                await unitOfWork.PermissionClaimService.RemovePermissionClaimsAsync(
                    UserId, cancellationToken);

                await unitOfWork.PermissionClaimService.AddPermissionClaimsFromRolesAsync(
                    UserId, new[] { Dto.RoleId }, cancellationToken);
            }

            return result;
        }
    }
}
