using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class ResetPasswordCommand : AuthCommand<bool>
    {
        public ResetPasswordDTO Dto { get; }

        public ResetPasswordCommand(ResetPasswordDTO dto)
        {
            Dto = dto;
        }

        public override async Task<bool> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.AuthenticationService.ResetPasswordAsync(Dto.Email, Dto.Token, Dto.NewPassword, cancellationToken);
        }
    }
}
