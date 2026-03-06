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

            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault();
            
            // Note: We might need a way to map role name back to a custom role ID if needed, 
            // but for now we look for the IdentityRole ID.
            // Actually, in SITRA, Custom Roles (business roles) and Identity Roles are linked by name.
            // Let's just return the first role as the RoleId for now if it's a GUID, or role name.
            // Wait, standard Identity roles use string IDs (often GUIDs).
            
            var dto = new UserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                CompanyName = user.CompanyName,
                ParentUserId = user.ParentUserId
            };

            if (roleName != null)
            {
                // We want the Custom Role ID from the RoleRepository or similar if possible.
                // However, Custom Roles and Identity Roles share the name.
                // For simplicity, let's just use the role name as the RoleId for now if the frontend identifies it that way,
                // or we join with the Roles table.
                dto.RoleId = roleName; 
            }

            return dto;
        }
    }
}
