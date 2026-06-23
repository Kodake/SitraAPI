using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class ReceivableRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<Receivable>(context, httpContextAccessor), IReceivableRepository
    {
        private new readonly HttpContext httpContext = httpContextAccessor.HttpContext!;


        public async Task<Receivable?> GetAllByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Receivables
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .Include(x => x.Invoice.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
                .Include(x => x.PaymentReceipts.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


    }
}





