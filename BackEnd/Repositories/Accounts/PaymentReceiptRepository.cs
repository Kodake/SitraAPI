using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
using BackEnd.Helpers;
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
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<PaymentReceipt>> GetAll(PaginationDTO paginationDTO, long receivableId)
        {
            var queryable = context.PaymentReceipts
                .AsNoTracking()
                .Include(x => x.Receivable)
                .Include(x => x.Receivable.SaleInvoice)
                .Include(x => x.Receivable.Buyer)
                .Where(x => x.ReceivableId == receivableId)
                .OrderBy(x => x.Id);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var payments = await queryable
                .Paginate(paginationDTO)
                .ToListAsync();

            return payments;
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
