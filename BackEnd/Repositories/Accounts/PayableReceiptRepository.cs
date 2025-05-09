using BackEnd.Context;
using BackEnd.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Accounts
{
    public class PayableReceiptRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PayableReceipt>(context, httpContextAccessor), IPayableReceiptRepository
    {
        public async new Task<PayableReceipt?> GetByIdAsync(long id)
        {
            return await context.PayableReceipts
                .Include(p => p.Payable)
                    .ThenInclude(x => x.Invoice)
                    .ThenInclude(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
