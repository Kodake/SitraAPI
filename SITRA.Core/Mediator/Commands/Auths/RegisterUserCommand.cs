using SITRA.Core.DTOs.Auths;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class RegisterUserCommand : IRequest<IdentityResult>
    {
        public CreateRegistrationUserDTO Dto { get; }

        public RegisterUserCommand(CreateRegistrationUserDTO dto)
        {
            Dto = dto;
        }
    }
}
