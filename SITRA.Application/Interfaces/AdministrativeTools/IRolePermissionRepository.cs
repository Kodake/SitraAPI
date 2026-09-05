using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IRolePermissionRepository : IRepositoryBase<RolePermission>
    {
        Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken);
        Task UpdateRolePermissionsAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken);
    }
}
