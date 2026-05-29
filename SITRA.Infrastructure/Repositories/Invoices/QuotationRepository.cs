using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Invoices;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Invoices
{
    public class QuotationRepository : RepositoryBase<Quotation>, IQuotationRepository
    {
        private new readonly HttpContext httpContext;
        public QuotationRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Quotations
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.InvoiceBuyer)
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.InvoiceBiller)
                        .ThenInclude(ib => ib.Biller)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.Invoice.InvoiceType == InvoiceType.Quotation)
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<List<Quotation>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Quotations.AsNoTracking()
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.InvoiceBuyer)
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.InvoiceBiller)
                        .ThenInclude(ib => ib.Biller)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.Invoice.InvoiceType == InvoiceType.Quotation)
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<Quotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Quotations
                .Include(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceItems)
                .Include(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceBuyer)
                .Include(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceBiller)
                        .ThenInclude(ib => ib.Biller)
                .Include(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}



