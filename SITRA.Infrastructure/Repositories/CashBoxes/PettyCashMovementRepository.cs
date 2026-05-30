using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.CashBoxes;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.CashBoxes
{
    public class PettyCashMovementRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : RepositoryBase<PettyCashMovement>(context, httpContextAccessor), IPettyCashMovementRepository
    {
        private new readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

        public async Task<List<PettyCashMovement>> GetAllAsync(PaginationDTO paginationDTO, Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var queryable = context.PettyCashMovements
                .Include(x => x.Invoice)
                .Include(x => x.PettyCash)
                .Where(x => x.PettyCashId == pettyCashId && x.Type == MovementType.Income);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PettyCashMovement>> GetReadOnlyAsync(PaginationDTO paginationDTO, Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var queryable = context.PettyCashMovements.AsNoTracking()
                .Include(x => x.Invoice)
                .Include(x => x.PettyCash)
                .Where(x => x.PettyCashId == pettyCashId && x.Type == MovementType.Income);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PettyCashMovement>> GetAllWithdrawalsAsync(PaginationDTO paginationDTO, Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var queryable = context.PettyCashMovements
                .Include(x => x.PettyCash)
                .Where(x => x.PettyCashId == pettyCashId && x.Type == MovementType.CashWithdrawals);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }



        public async Task<double> GetTotalAmountPaidAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            return await context.PettyCashMovements
                .Include(x => x.Invoice)
                .Where(x => x.PettyCashId == pettyCashId &&
                            x.Invoice != null &&
                            x.Invoice.Receivable == null &&
                            x.Invoice.SpotPayment == null)
                .SumAsync(x => x.Amount, cancellationToken);
        }

    }
}




