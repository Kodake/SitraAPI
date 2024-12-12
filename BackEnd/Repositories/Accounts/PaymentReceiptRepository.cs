using BackEnd.Context;
using BackEnd.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Accounts
{
    public class PaymentReceiptRepository : IPaymentReceiptRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public PaymentReceiptRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<PaymentReceipt?> GetById(long id)
        {
            return await context.PaymentReceipts
                .Include(p => p.Receivable)
                    .ThenInclude(x => x.SaleInvoice)
                    .ThenInclude(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long> Create(long receivableId, PaymentReceipt paymentReceipt)
        {
            paymentReceipt.ReceivableId = receivableId;
            await context.PaymentReceipts.AddAsync(paymentReceipt);
            await context.SaveChangesAsync();
            return paymentReceipt.Id;
        }

        public async Task Deactivate(long id)
        {
            var paymentReceipt = await context.PaymentReceipts.FirstOrDefaultAsync(x => x.Id == id);
            if (paymentReceipt != null)
            {
                paymentReceipt.IsActive = false;
                await context.SaveChangesAsync();
            }
        }
    }
}
