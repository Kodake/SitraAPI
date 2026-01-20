using SITRA.Core.DTOs.Auths;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class RegisterUserCommand : IRequest<IdentityResult>, IIdempotentCommand
    {
        public CreateRegistrationUserDTO Dto { get; }

        public RegisterUserCommand(CreateRegistrationUserDTO dto)
        {
            Dto = dto;
        }
    }
}
