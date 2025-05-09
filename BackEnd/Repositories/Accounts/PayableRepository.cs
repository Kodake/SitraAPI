using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Accounts;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Accounts
{
    public class PayableRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<Payable>(context, httpContextAccessor), IPayableRepository
    {
        private new readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

        public async new Task<List<Payable>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Payables
                .Include(x => x.PayableReceipts)
                .Include(x => x.Invoice)
                .Include(x => x.Supplier);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Payable?> GetAllByIdAsync(long id)
        {
            return await context.Payables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier)
                .Include(x => x.Invoice.Biller)
                .Include(x => x.PayableReceipts.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Payable?> GetPaysByIdAsync(long id)
        {
            return await context.Payables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier)
                .Include(x => x.PayableReceipts
                    .Where(x => x.IsActive)
                    .OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == AccountStatus.Paid);
        }
    }
}
