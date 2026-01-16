using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface IQuotationRepository : IRepositoryBase<Quotation>
    {
        new Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO);
        new Task<Quotation?> GetByIdAsync(long id);
    }
}
