using AutoMapper;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Mediator.Commands.Auths;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request.Dto);
            user.TenantId = Guid.NewGuid();

            return await _userManager.CreateAsync(user, request.Dto.Password);
        }
    }
}
