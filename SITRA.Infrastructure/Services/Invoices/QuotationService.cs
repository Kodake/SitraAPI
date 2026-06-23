using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Invoices
{
    public class QuotationService : IQuotationService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public QuotationService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Quotations.AsNoTracking()
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.InvoiceBuyer)
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.InvoiceBiller)
                        .ThenInclude(ib => ib.Biller)
                .Include(x => x.Buyer)
                .Where(x => x.Invoice.InvoiceType == InvoiceType.Quotation);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(q =>
                    (q.QuotationNumber != null && q.QuotationNumber.ToLower().Contains(searchTerm)) ||
                    (q.Buyer != null && q.Buyer.Name != null && q.Buyer.Name.ToLower().Contains(searchTerm))
                );
            }

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .Where(x => x.Invoice.InvoiceType == InvoiceType.Quotation)
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
