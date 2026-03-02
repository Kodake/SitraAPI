using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Mediator.Queries.Tenants
{
    public record GetAllTenantUsersQuery(Guid TenantId) : IRequest<IEnumerable<UserDTO>>;

    public class GetAllTenantUsersHandler : IRequestHandler<GetAllTenantUsersQuery, IEnumerable<UserDTO>>
    {
        private readonly UserManager<User> _userManager;

        public GetAllTenantUsersHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDTO>> Handle(GetAllTenantUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users
                .Where(u => u.TenantId == request.TenantId)
                .ToListAsync(cancellationToken);

            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                Email = u.Email!,
                CompanyName = u.CompanyName,
                ParentUserId = u.ParentUserId
            });
        }
    }
}
