using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.Interfaces.Services.Invoices
{
    public interface IQuotationService
    {
        Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
    }
}
