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



        public async new Task<Quotation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Quotations
                .Include(x => x.Invoice)
                    .ThenInclude(x => x!.InvoiceItems)
                .Include(x => x.Invoice)
                    .ThenInclude(x => x!.Buyer)
                .Include(x => x.Invoice)
                    .ThenInclude(x => x!.Biller)
                .Include(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, string quotationNumber, CancellationToken cancellationToken = default)
        {
            return await context.Quotations.AnyAsync(x => x.Id != id && x.QuotationNumber.ToLower() == quotationNumber.ToLower(), cancellationToken);
        }
    }
}



