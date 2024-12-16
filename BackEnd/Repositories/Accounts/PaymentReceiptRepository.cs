using BackEnd.Context;
using BackEnd.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Accounts
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
                    .ThenInclude(x => x.SaleInvoice)
                    .ThenInclude(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
