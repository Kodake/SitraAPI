using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface IQuotationRepository : IRepositoryBase<Quotation>
    {
        new Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        new Task<Quotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
