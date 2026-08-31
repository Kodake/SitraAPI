using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Accounts;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Accounts
{
    public class PayableRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<Payable>(context, httpContextAccessor), IPayableRepository
    {
        private new readonly HttpContext httpContext = httpContextAccessor.HttpContext!;


        public async Task<Payable?> GetAllByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Payables
                .Include(x => x.Invoice)
                    .ThenInclude(i => i!.Biller)
                .Include(x => x.Supplier)
                .Include(x => x.PayableReceipts.OrderByDescending(p => p.Id))
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


    }
}





