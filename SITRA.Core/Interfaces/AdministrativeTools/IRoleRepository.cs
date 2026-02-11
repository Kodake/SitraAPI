using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IRoleRepository : IRepositoryBase<Role>
    {
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
