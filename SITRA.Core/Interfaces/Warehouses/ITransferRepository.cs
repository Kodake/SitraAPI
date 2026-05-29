using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface ITransferRepository : IRepositoryBase<Transfer>
    {
        new Task<Transfer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        new Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<List<Transfer>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
