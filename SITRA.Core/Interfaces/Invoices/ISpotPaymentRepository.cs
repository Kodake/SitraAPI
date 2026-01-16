using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ISpotPaymentRepository : IRepositoryBase<SpotPayment>
    {
        new Task<List<SpotPayment>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<SpotPayment?> GetByIdAsync(long id);
    }
}
