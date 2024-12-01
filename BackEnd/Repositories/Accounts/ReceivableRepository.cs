using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
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
            var queryable = context.Receivables.AsQueryable();

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<long> Create(Receivable receivable)
        {
            context.Receivables.Add(receivable);
            await context.SaveChangesAsync();
            return receivable.Id;
        }

        public async Task<bool> Exists(long id)
        {
            return await context.Receivables.AnyAsync(x => x.Id == id);
        }
    }
}
