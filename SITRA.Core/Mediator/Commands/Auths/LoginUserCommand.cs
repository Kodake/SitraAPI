using MediatR;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class LoginUserCommand : IRequest<TokenResponseDTO?>
    {
        public CreateLoginUserDTO Dto { get; }

        public LoginUserCommand(CreateLoginUserDTO dto)
        {
            Dto = dto;
        }
    }
}
