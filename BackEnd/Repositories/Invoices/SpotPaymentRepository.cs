using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Invoices
{
    public class SpotPaymentRepository : RepositoryBase<SpotPayment>, ISpotPaymentRepository
    {
        private new readonly HttpContext httpContext;
        public SpotPaymentRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<SpotPayment>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.SpotPayments
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async new Task<SpotPayment?> GetByIdAsync(long id)
        {
            return await context.SpotPayments
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
