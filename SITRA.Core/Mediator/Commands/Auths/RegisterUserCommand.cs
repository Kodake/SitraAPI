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

        public override Task<IdentityResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.UserService.RegisterAsync(Dto, cancellationToken);
        }
    }
}
