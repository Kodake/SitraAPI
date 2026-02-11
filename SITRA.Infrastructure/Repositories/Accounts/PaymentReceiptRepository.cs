using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class PaymentReceiptRepository : RepositoryBase<PaymentReceipt>, IPaymentReceiptRepository
    {
        public PaymentReceiptRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        { }

        public async new Task<PaymentReceipt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.PaymentReceipts
                .Include(p => p.Receivable)
                    .ThenInclude(x => x.Invoice)
                    .ThenInclude(x => x.InvoiceBuyer)
                        .ThenInclude(ib => ib.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}





