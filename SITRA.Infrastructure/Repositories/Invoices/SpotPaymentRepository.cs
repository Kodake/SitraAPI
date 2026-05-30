using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Invoices;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Invoices
{
    public class SpotPaymentRepository : RepositoryBase<SpotPayment>, ISpotPaymentRepository
    {
        private new readonly HttpContext httpContext;
        public SpotPaymentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }



        public async new Task<SpotPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.SpotPayments
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}




