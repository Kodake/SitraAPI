using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IPermissionClaimService
    {
        Task AddPermissionClaimsFromRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken);
        Task RemovePermissionClaimsAsync(string userId, CancellationToken cancellationToken);
    }
}
