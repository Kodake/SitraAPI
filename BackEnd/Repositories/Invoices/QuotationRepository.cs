using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Invoices
{
    public class QuotationRepository : RepositoryBase<Quotation>, IQuotationRepository
    {
        private new readonly HttpContext httpContext;
        public QuotationRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Quotation>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.Quotations
                .Include(x => x.SaleInvoice)
                .Include(x => x.Buyer);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async new Task<Quotation?> GetByIdAsync(long id)
        {
            return await context.Quotations
                .Include(x => x.SaleInvoice)
                    .ThenInclude(x => x.SaleInvoiceItems)
                .Include(x => x.Buyer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
