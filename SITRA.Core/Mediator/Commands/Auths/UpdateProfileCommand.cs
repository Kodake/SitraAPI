using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class UpdateProfileCommand : AuthCommand<bool>
    {
        public UpdateProfileNameDTO Dto { get; }

        public UpdateProfileCommand(UpdateProfileNameDTO dto)
        {
            Dto = dto;
        }

        public override Task<bool> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.UserRoleService.UpdateProfileNameAsync(Dto, cancellationToken);
        }
    }
}
