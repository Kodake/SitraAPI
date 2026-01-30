using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Mediator.Commands.Auths;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SITRA.Core.Interfaces.IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(UserManager<User> userManager, IMapper mapper, SITRA.Core.Interfaces.IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IdentityResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request.Dto);

            // Create and Save Tenant
            var tenant = new SITRA.Core.Entities.Tenants.Tenant
            {
                TenantId = Guid.NewGuid(),
                Name = request.Dto.CompanyName,
                Plan = request.Dto.Plan
            };

            await _unitOfWork.Repository<SITRA.Core.Entities.Tenants.Tenant>().CreateAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            user.TenantId = tenant.TenantId;

            return await _userManager.CreateAsync(user, request.Dto.Password);
        }
    }
}
