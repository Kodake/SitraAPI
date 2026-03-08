using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IPermissionClaimService
    {
        Task AddPermissionClaimsFromRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken);
        Task SyncPermissionClaimsAsync(string userId, IEnumerable<Guid> newRoleIds, CancellationToken cancellationToken);
    }
}
