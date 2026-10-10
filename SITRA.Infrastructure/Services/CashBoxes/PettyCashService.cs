using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.CashBoxes
{
    public class PettyCashService : IPettyCashService
    {
        private readonly ApplicationDbContext _context;

        public PettyCashService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<object, double>> GetPaymentMethodSumsAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            var payments = await _context.PettyCashMovements
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
            var payments = await _context.PettyCashMovements
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
        public async Task<double> GetTotalWithdrawalsAsync(Guid pettyCashId, CancellationToken cancellationToken = default)
        {
            return await _context.PettyCashMovements
                .Where(x => x.PettyCashId == pettyCashId && x.Type == MovementType.CashWithdrawals)
                .SumAsync(x => x.Amount, cancellationToken);
        }
    }
}
