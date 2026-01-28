using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ISpotPayableRepository : IRepositoryBase<SpotPayable>
    {
        new Task<List<SpotPayable>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<SpotPayable?> GetByIdAsync(Guid id);
    }
}
