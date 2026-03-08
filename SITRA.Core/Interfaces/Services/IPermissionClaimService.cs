using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Interfaces.Services
{
    public interface IPermissionClaimService
    {
        Task AddPermissionClaimsFromRolesAsync(User user, IEnumerable<Guid> roleIds, CancellationToken cancellationToken);
    }
}
