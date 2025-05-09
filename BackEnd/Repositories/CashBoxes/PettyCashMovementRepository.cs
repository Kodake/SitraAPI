using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.CashBoxes;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.CashBoxes
{
    public class PettyCashMovementRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PettyCashMovement>(context, httpContextAccessor), IPettyCashMovementRepository
    {
        private new readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

        public async Task<List<PettyCashMovement>> GetAllAsync(PaginationDTO paginationDTO, long pettyCashId)
        {
            var queryable = context.PettyCashMovements
                .Include(x => x.Invoice)
                .Include(x => x.PettyCash)
                .Where(x => x.PettyCashId == pettyCashId);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<double> GetTotalAmountPaidAsync(long pettyCashId)
        {
            return await context.PettyCashMovements
                .Include(x => x.Invoice)
                .Where(x => x.PettyCashId == pettyCashId &&
                            x.Invoice.Receivable == null &&
                            x.Invoice.SpotPayment == null 
                            )
                .SumAsync(x => x.Amount);
        }
    }
}
