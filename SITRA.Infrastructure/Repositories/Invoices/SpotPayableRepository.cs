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
    public class SpotPayableRepository : RepositoryBase<SpotPayable>, ISpotPayableRepository
    {
        private new readonly HttpContext httpContext;
        public SpotPayableRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<SpotPayable>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.SpotPayables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<SpotPayable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.SpotPayables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}




