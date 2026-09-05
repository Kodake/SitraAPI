using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class ForgotPasswordCommand : AuthCommand<bool>
    {
        public ForgotPasswordDTO Dto { get; }

        public ForgotPasswordCommand(ForgotPasswordDTO dto)
        {
            Dto = dto;
        }

        public override async Task<bool> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return await unitOfWork.AuthenticationService.ForgotPasswordAsync(Dto.Email, cancellationToken);
        }
    }
}
