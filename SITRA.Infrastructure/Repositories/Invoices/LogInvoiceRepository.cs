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
    internal class LogInvoiceRepository : RepositoryBase<Invoice>, ILogInvoiceRepository
    {
        private new readonly HttpContext httpContext;
        public LogInvoiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Invoice>> GetAllAsync(PaginationDTO paginationDTO, string? dgiiStatus = null, CancellationToken cancellationToken = default)
        {
            var queryable = context.Invoices.AsQueryable();

            if (!string.IsNullOrEmpty(dgiiStatus))
            {
                queryable = queryable.Where(i => i.DgiiStatus == dgiiStatus);
            }

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderByDescending(i => i.CreatedAt)
                .ThenByDescending(i => i.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
