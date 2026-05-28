using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Interfaces.Generics;

namespace SITRA.Core.Interfaces.Invoices
{
    public interface ILogInvoiceRepository : IRepositoryBase<Invoice>
    {
        Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO, string? dgiiStatus = null, CancellationToken cancellationToken = default);
    }
}
