using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Core.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class PaymentReceiptRepository : RepositoryBase<PaymentReceipt>, IPaymentReceiptRepository
    {
        public PaymentReceiptRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        { }

        public async new Task<PaymentReceipt?> GetByIdAsync(long id)
        {
            return await context.PaymentReceipts
                .Include(p => p.Receivable)
                    .ThenInclude(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceBuyer)
                        .ThenInclude(ib => ib.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}





