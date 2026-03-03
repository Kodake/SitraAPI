using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class LoginUserCommand : AuthCommand<TokenResponseDTO?>
    {
        public CreateLoginUserDTO Dto { get; }

        public LoginUserCommand(CreateLoginUserDTO dto)
        {
            Dto = dto;
        }

        public override Task<TokenResponseDTO?> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.UserService.LoginAsync(Dto, cancellationToken);
        }
    }
}
