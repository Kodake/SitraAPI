using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.AdministrativeTools
{
    public class RolePermissionRepository : RepositoryBase<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken)
        {
            return await context.TenantRolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateRolePermissionsAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken)
        {
            var currentPermissions = await context.TenantRolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);

            context.TenantRolePermissions.RemoveRange(currentPermissions);

            var newPermissions = permissionIds.Select(pId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pId
            });

            await context.TenantRolePermissions.AddRangeAsync(newPermissions, cancellationToken);
        }
    }
}
