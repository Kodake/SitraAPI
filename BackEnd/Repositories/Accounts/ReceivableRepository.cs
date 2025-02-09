using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Accounts
{
    public class ReceivableRepository : RepositoryBase<Receivable>, IReceivableRepository
    {
        private new readonly HttpContext httpContext;
        public ReceivableRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Receivable>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Receivables
                .Include(x => x.PaymentReceipts)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Receivable?> GetAllByIdAsync(long id)
        {
            return await context.Receivables
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .Include(x => x.Invoice.Biller)
                .Include(x => x.PaymentReceipts.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Receivable?> GetPaysByIdAsync(long id)
        {
            return await context.Receivables
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .Include(x => x.PaymentReceipts
                    .Where(x => x.IsActive)
                    .OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == AccountStatus.Paid);
        }
    }
}
