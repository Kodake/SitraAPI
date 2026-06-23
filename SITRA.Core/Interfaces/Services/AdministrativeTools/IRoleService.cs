using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.Core.Interfaces.Services.AdministrativeTools
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
