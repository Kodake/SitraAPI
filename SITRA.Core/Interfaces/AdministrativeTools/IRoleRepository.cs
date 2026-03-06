using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.AdministrativeTools
{
    public interface IRoleRepository : IRepositoryBase<Role>
    {
        new Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        new Task<List<Role>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Role>> GetAll(CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
