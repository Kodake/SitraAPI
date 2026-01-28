using SITRA.Core.DTOs;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Warehouses
{
    public interface ITransferRepository : IRepositoryBase<Transfer>
    {
        new Task<Transfer?> GetByIdAsync(Guid id);
        new Task<List<Transfer>> GetAllAsync(PaginationDTO paginationDTO);
    }
}
