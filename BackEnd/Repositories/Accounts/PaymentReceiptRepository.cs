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

        public async Task<long> Create(PaymentReceipt paymentReceipt)
        {
            await context.PaymentReceipts.AddAsync(paymentReceipt);
            await context.SaveChangesAsync();
            return paymentReceipt.Id;
        }

        public async Task<List<PaymentReceipt>> GetAll(PaginationDTO paginationDTO, long receivableId)
        {
            var queryable = context.PaymentReceipts
                .Include(x => x.Receivable)
                    .ThenInclude(x => x.SaleInvoice)
                    .ThenInclude(x => x.Buyer)
                .Where(x => x.ReceivableId == receivableId)
                .OrderBy(x => x.Id);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var payments = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.Receivable)
                .ToListAsync();

            return payments;
        }

        public async Task<PaymentReceipt?> GetByIdAndReceivableId(long id, long receivableId)
        {
            return await context.PaymentReceipts
                .Include(p => p.Receivable)
                .FirstOrDefaultAsync(x => x.Id == id && x.ReceivableId == receivableId);
        }
    }
}
