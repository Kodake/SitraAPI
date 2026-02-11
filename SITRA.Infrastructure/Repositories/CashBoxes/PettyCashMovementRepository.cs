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

        public async Task<double> GetTotalWithdrawalsAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            return await context.PettyCashMovements
                .Where(x => x.PettyCashId == pettyCashId && x.Type == MovementType.CashWithdrawals)
                .SumAsync(x => x.Amount, cancellationToken);
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

        public async Task<Dictionary<object, double>> GetPaymentMethodSumsAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var payments = await context.PettyCashMovements
                .Include(x => x.Invoice)
                .Where(m => m.PettyCashId == pettyCashId &&
                            m.Invoice != null &&
                            m.Invoice.SpotPayment != null)
                .SelectMany(m => m.Invoice!.SpotPayment!.Payments)
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new
                {
                    PaymentMethod = g.Key,
                    TotalAmount = g.Sum(p => p.Amount)
                })
                .ToListAsync(cancellationToken);

            var allMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>();

            var result = allMethods.ToDictionary(
                method => (object)method,
                method => payments.FirstOrDefault(p => p.PaymentMethod == method)?.TotalAmount ?? 0
            );

            result["Total"] = result.Values.Sum();

            return result;
        }

        public async Task<List<PaymentMethodAmountListDTO>> GetPaymentMethodAmountListAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var payments = await context.PettyCashMovements
                .Include(x => x.Invoice)
                .Where(m => m.PettyCashId == pettyCashId &&
                            m.Invoice != null &&
                            m.Invoice.SpotPayment != null)
                .SelectMany(m => m.Invoice!.SpotPayment!.Payments)
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new
                {
                    PaymentMethod = g.Key,
                    TotalAmount = g.Sum(p => p.Amount)
                })
                .ToListAsync(cancellationToken);

            var allMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>();

            var result = allMethods.Select(method => new PaymentMethodAmountListDTO
            {
                MethodType = method,
                Amount = payments.FirstOrDefault(p => p.PaymentMethod == method)?.TotalAmount ?? 0
            }).ToList();

            return result;
        }
    }
}




