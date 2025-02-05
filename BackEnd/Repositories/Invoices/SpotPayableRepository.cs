using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Invoices;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Invoices
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

        public async new Task<SpotPayable?> GetByIdAsync(long id)
        {
            return await context.SpotPayables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
