using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Accounts
{
    public class ReceivableRepository : IReceivableRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public ReceivableRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Receivable>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Receivables
                .Include(x => x.PaymentReceipts)
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Receivable?> GetAllById(long id)
        {
            return await context.Receivables
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer)
                .Include(x => x.PaymentReceipts.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Receivable?> GetPaysById(long id)
        {
            return await context.Receivables
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer)
                .Include(x => x.PaymentReceipts
                    .Where(x => x.IsActive)
                    .OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == AccountStatus.Paid);
        }

        public async Task<Receivable?> GetById(long id)
        {
            return await context.Receivables
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<long> Create(Receivable receivable)
        {
            context.Receivables.Add(receivable);
            await context.SaveChangesAsync();
            return receivable.Id;
        }

        public async Task Update(Receivable receivable)
        {
            context.Update(receivable);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Exists(long id)
        {
            return await context.Receivables.AnyAsync(x => x.Id == id);
        }
    }
}
