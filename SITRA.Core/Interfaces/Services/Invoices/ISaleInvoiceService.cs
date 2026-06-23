using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.Interfaces.Services.Invoices
{
    public interface ISaleInvoiceService
    {
        Task<List<Invoice>> GetSalesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO, CancellationToken cancellationToken = default);
        Task<List<Invoice>> GetSalesReportByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    }
}
