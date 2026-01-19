using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Accounts;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class ReceivableRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<Receivable>(context, httpContextAccessor), IReceivableRepository
    {
        private new readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

        public async new Task<List<Receivable>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Receivables
                .Include(x => x.PaymentReceipts)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Receivable?> GetAllByIdAsync(long id)
        {
            return await context.Receivables
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .Include(x => x.Invoice.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
                .Include(x => x.PaymentReceipts.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Receivable?> GetPaysByIdAsync(long id)
        {
            return await context.Receivables
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .Include(x => x.PaymentReceipts
                    .Where(x => x.IsActive)
                    .OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == AccountStatus.Paid);
        }
    }
}




