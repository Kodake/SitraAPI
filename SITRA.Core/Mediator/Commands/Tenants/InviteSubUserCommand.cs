using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.Tenants;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Tenants
{
    public record InviteSubUserCommand(InviteSubUserDTO DTO, Guid TenantId, string ParentUserId) : IRequest<string?>;

    public class InviteSubUserHandler : IRequestHandler<InviteSubUserCommand, string?>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanService _planService;

        public InviteSubUserHandler(UserManager<User> userManager, IUnitOfWork unitOfWork, IPlanService planService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _planService = planService;
        }

        public async Task<string?> Handle(InviteSubUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Check Plan Limits
            var tenant = await _unitOfWork.Repository<Tenant>().GetByIdAsync(request.TenantId, cancellationToken);
            if (tenant == null) return null;

            var maxUsers = _planService.GetMaxUsers(tenant.Plan);
            var currentUsersCount = await _userManager.Users.CountAsync(u => u.TenantId == request.TenantId, cancellationToken);

            if (currentUsersCount >= maxUsers) return null;

            // 2. Create Pending User
            var invitationToken = Guid.NewGuid().ToString("N");
            var user = new User
            {
                UserName = $"pending_{invitationToken}",
                Email = request.DTO.Email,
                TenantId = request.TenantId,
                ParentUserId = request.ParentUserId,
                InvitationToken = invitationToken,
                InvitationExpiry = DateTimeOffset.UtcNow.AddDays(7)
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) return null;

            // 3. Assign Role immediately (optional but recommended)
            await _userManager.AddToRoleAsync(user, request.DTO.RoleName);

            return invitationToken;
        }
    }
}
