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

        public override Task<IdentityResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.UserService.RegisterAssociatedUserAsync(Dto, cancellationToken);
        }
    }
}
