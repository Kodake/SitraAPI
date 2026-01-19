using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Accounts;
using Microsoft.EntityFrameworkCore;

namespace SITRA.Infrastructure.Repositories.Accounts
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
                .Include(x => x.Invoice.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
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




