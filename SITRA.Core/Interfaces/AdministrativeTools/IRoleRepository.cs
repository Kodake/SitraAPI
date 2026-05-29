using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IRoleRepository : IRepositoryBase<Role>
    {
        new Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        new Task<List<Role>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Role>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Role>> GetAll(bool? hasPermissions = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default);
    }
}
