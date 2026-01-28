using Microsoft.AspNetCore.Http;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Invoices;
using Microsoft.EntityFrameworkCore;

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

        public async new Task<List<SpotPayable>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.SpotPayables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async new Task<SpotPayable?> GetByIdAsync(Guid id)
        {
            return await context.SpotPayables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}




