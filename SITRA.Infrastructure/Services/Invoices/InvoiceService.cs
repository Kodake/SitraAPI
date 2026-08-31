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
            IQueryable<Invoice> queryable = _context.Invoices
                .Where(x => x.InvoiceType == InvoiceType.Sale)
                .Include(x => x.Buyer)
                .Include(x => x.Biller)
                .Include(x => x.FiscalSupport);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.FiscalSupportNumber != null && x.FiscalSupportNumber.ToLower().Contains(searchTerm)) ||
                    (x.ModifiedNcfNumber != null && x.ModifiedNcfNumber.ToLower().Contains(searchTerm)) ||
                    (x.Buyer != null && x.Buyer.Name.ToLower().Contains(searchTerm)) ||
                    (x.Biller != null && x.Biller.Name.ToLower().Contains(searchTerm))
                );
            }

            await _httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.FiscalSupport != null
                    && x.FiscalSupport.FiscalSupportType != EcfType.E31
                    && x.FiscalSupport.FiscalSupportType != EcfType.E33
                    && x.FiscalSupport.FiscalSupportType != EcfType.E34
                    && x.FiscalSupport.FiscalSupportType != EcfType.E41
                    && x.FiscalSupport.FiscalSupportType != EcfType.B01
                    && x.FiscalSupport.FiscalSupportType != EcfType.B03
                    && x.FiscalSupport.FiscalSupportType != EcfType.B04
                    && x.FiscalSupport.FiscalSupportType != EcfType.B11
                    && !_context.Invoices.Any(y => y.ModifiedNcfNumber == x.FiscalSupportNumber && x.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado")
                    && !(x.ModifiedNcfNumber != null && x.DgiiStatus == "Aceptado"))
                .OrderByDescending(i => i.CreatedAt)
                .ThenByDescending(i => i.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetValidPurchaseInvoicesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            IQueryable<Invoice> queryable = _context.Invoices.AsNoTracking()
                .Where(x => x.InvoiceType == InvoiceType.Purchase)
                .Include(x => x.Buyer)
                .Include(x => x.FiscalSupport);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.FiscalSupportNumber != null && x.FiscalSupportNumber.ToLower().Contains(searchTerm)) ||
                    (x.ModifiedNcfNumber != null && x.ModifiedNcfNumber.ToLower().Contains(searchTerm)) ||
                    (x.Buyer != null && x.Buyer.Name.ToLower().Contains(searchTerm)) ||
                    (x.Biller != null && x.Biller.Name.ToLower().Contains(searchTerm))
                );
            }

            await _httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .Where(x => x.InvoiceType == InvoiceType.Purchase
                    && x.FiscalSupport != null
                    && x.FiscalSupport.FiscalSupportType != EcfType.E32
                    && x.FiscalSupport.FiscalSupportType != EcfType.E33
                    && x.FiscalSupport.FiscalSupportType != EcfType.E34
                    && x.FiscalSupport.FiscalSupportType != EcfType.E43
                    && x.FiscalSupport.FiscalSupportType != EcfType.E44
                    && x.FiscalSupport.FiscalSupportType != EcfType.E45
                    && x.FiscalSupport.FiscalSupportType != EcfType.E46
                    && x.FiscalSupport.FiscalSupportType != EcfType.E47
                    && x.FiscalSupport.FiscalSupportType != EcfType.B02
                    && x.FiscalSupport.FiscalSupportType != EcfType.B03
                    && x.FiscalSupport.FiscalSupportType != EcfType.B04
                    && x.FiscalSupport.FiscalSupportType != EcfType.B13
                    && x.FiscalSupport.FiscalSupportType != EcfType.B14
                    && x.FiscalSupport.FiscalSupportType != EcfType.B15
                    && x.FiscalSupport.FiscalSupportType != EcfType.B16
                    && x.FiscalSupport.FiscalSupportType != EcfType.B17
                    && !_context.Invoices.Any(y => y.ModifiedNcfNumber == x.FiscalSupportNumber && x.FiscalSupportNumber != null && y.DgiiStatus == "Aceptado")
                    && !(x.ModifiedNcfNumber != null && x.DgiiStatus == "Aceptado"))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<SpotPayable>> GetValidSpotPayablesAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            IQueryable<SpotPayable> queryable = _context.SpotPayables
                .Include(x => x.Invoice)
                .Include(x => x.Supplier);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.Invoice != null && x.Invoice.FiscalSupportNumber != null && x.Invoice.FiscalSupportNumber.ToLower().Contains(searchTerm)) ||
                    (x.Supplier != null && x.Supplier.Name.ToLower().Contains(searchTerm))
                );
            }

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
            IQueryable<SpotPayment> queryable = _context.SpotPayments
                .Include(x => x.Payments)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer);

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.Invoice != null && x.Invoice.FiscalSupportNumber != null && x.Invoice.FiscalSupportNumber.ToLower().Contains(searchTerm)) ||
                    (x.Buyer != null && x.Buyer.Name.ToLower().Contains(searchTerm))
                );
            }

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
