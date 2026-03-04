using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Mediator.Queries.Tenants
{
    public record GetUserByIdQuery(Guid TenantId, string UserId) : IRequest<UserDTO?>;

    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDTO?>
    {
        private readonly UserManager<User> _userManager;

        public GetUserByIdHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDTO?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == request.TenantId, cancellationToken);

            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                CompanyName = user.CompanyName,
                ParentUserId = user.ParentUserId
            };
        }
    }
}
