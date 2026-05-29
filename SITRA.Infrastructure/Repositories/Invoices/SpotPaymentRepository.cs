using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Invoices;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Invoices
{
    public class SpotPaymentRepository : RepositoryBase<SpotPayment>, ISpotPaymentRepository
    {
        private new readonly HttpContext httpContext;
        public SpotPaymentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<SpotPayment>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.SpotPayments
                .Include(x => x.Payments)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.Invoice != null && !context.Invoices.Any(y => y.ModifiedNcfNumber == x.Invoice.FiscalSupportNumber && x.Invoice.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado"))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<List<SpotPayment>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.SpotPayments.AsNoTracking()
                .Include(x => x.Payments)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.Invoice != null && !context.Invoices.Any(y => y.ModifiedNcfNumber == x.Invoice.FiscalSupportNumber && x.Invoice.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado"))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<SpotPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.SpotPayments
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}




