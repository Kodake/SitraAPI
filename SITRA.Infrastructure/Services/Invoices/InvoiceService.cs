using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Services;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContext _httpContext;

        public InvoiceService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Invoice>> GetValidSaleInvoicesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Invoices.AsNoTracking()
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Include(x => x.InvoiceBiller)
                    .ThenInclude(ib => ib.Biller)
                .Include(x => x.InvoiceFiscalSupport);

            await _httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.InvoiceType == InvoiceType.Sale
                    && x.InvoiceFiscalSupport != null
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E31
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E33
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E34
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E41
                    && !_context.Invoices.Any(y => y.ModifiedNcfNumber == x.FiscalSupportNumber && x.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado")
                    && !(x.ModifiedNcfNumber != null && x.DgiiStatus == "Aceptado"))
                .OrderByDescending(i => i.CreatedAt)
                .ThenByDescending(i => i.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetValidPurchaseInvoicesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Invoices.AsNoTracking()
                .Include(x => x.InvoiceBuyer)
                    .ThenInclude(ib => ib.Buyer)
                .Include(x => x.InvoiceFiscalSupport);

            await _httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.InvoiceType == InvoiceType.Purchase
                    && x.InvoiceFiscalSupport != null
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E32
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E33
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E34
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E43
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E44
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E45
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E46
                    && x.InvoiceFiscalSupport.EcfType != EcfType.E47
                    && !_context.Invoices.Any(y => y.ModifiedNcfNumber == x.FiscalSupportNumber && x.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado")
                    && !(x.ModifiedNcfNumber != null && x.DgiiStatus == "Aceptado"))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<SpotPayable>> GetValidSpotPayablesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.SpotPayables.AsNoTracking()
                .Include(x => x.Invoice)
                .Include(x => x.Supplier);

            await _httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.Invoice != null && !_context.Invoices.Any(y => y.ModifiedNcfNumber == x.Invoice.FiscalSupportNumber && x.Invoice.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado"))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SpotPayment>> GetValidSpotPaymentsAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.SpotPayments.AsNoTracking()
                .Include(x => x.Payments)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer);

            await _httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.Invoice != null && !_context.Invoices.Any(y => y.ModifiedNcfNumber == x.Invoice.FiscalSupportNumber && x.Invoice.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado"))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
