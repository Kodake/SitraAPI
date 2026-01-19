using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Core.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class PayableReceiptRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PayableReceipt>(context, httpContextAccessor), IPayableReceiptRepository
    {
        public async new Task<PayableReceipt?> GetByIdAsync(long id)
        {
            return await context.PayableReceipts
                .Include(p => p.Payable)
                    .ThenInclude(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceBuyer)
                        .ThenInclude(ib => ib.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}





