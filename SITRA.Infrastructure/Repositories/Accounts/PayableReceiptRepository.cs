using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class PayableReceiptRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PayableReceipt>(context, httpContextAccessor), IPayableReceiptRepository
    {
        public async new Task<PayableReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.PayableReceipts
                .Include(p => p.Payable)
                    .ThenInclude(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceBuyer)
                        .ThenInclude(ib => ib.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}





