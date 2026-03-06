using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using AutoMapper;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class UpdateUserRoleCommand : AuthCommand<IdentityResult>
    {
        public string UserId { get; }
        public UpdateUserRoleDTO Dto { get; }

        public UpdateUserRoleCommand(string userId, UpdateUserRoleDTO dto)
        {
            UserId = userId;
            Dto = dto;
        }

        public override Task<IdentityResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.UserService.UpdateUserRoleAsync(UserId, Dto, cancellationToken);
        }
    }
}
