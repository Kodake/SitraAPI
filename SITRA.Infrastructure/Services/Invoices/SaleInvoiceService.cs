using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Infrastructure.Persistence;
using System.Linq;

namespace SITRA.Infrastructure.Services.Invoices
{
    public class SaleInvoiceService : ISaleInvoiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public SaleInvoiceService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<Invoice>> GetSalesByDateRangeAsync(DateTime from, DateTime to, PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            var queryable = _context.Invoices.AsNoTracking()
                .Include(x => x.Buyer)
                .Include(x => x.Biller)
                .Where(x =>
                    x.InvoiceType == InvoiceType.Sale &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to &&
                    (x.SpotPayment != null || x.Receivable != null)
                );

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.FiscalSupportNumber != null && x.FiscalSupportNumber.ToLower().Contains(searchTerm)) ||
                    (x.ModifiedNcfNumber != null && x.ModifiedNcfNumber.ToLower().Contains(searchTerm)) ||
                    (x.Buyer != null && x.Buyer.Name.ToLower().Contains(searchTerm)) ||
                    (x.Biller != null && x.Biller.Name.ToLower().Contains(searchTerm))
                );
            }

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetSalesReportByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            to = to.Date.AddDays(1).AddTicks(-1);

            return await _context.Invoices.AsNoTracking()
                .Include(x => x.Buyer)
                .Include(x => x.Biller)
                .Where(x =>
                    x.InvoiceType == InvoiceType.Sale &&
                    x.CreatedAt >= from &&
                    x.CreatedAt <= to &&
                    (x.SpotPayment != null || x.Receivable != null)
                ).ToListAsync(cancellationToken);
        }
    }
}
