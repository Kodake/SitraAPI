using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Accounts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Accounts;
using SITRA.Infrastructure.Persistence;
using System.Linq;

namespace SITRA.Infrastructure.Services.Accounts
{
    public class ReceivableService : IReceivableService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public ReceivableService(
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<Receivable?> GetPaysByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.ReceivableRepository.GetAllByIdAsync(id, cancellationToken);
            if (item != null && item.Status == AccountStatus.Paid)
            {
                item.PaymentReceipts = item.PaymentReceipts.Where(x => x.IsActive).OrderByDescending(p => p.Id).ToList();
                return item;
            }
            return null;
        }

        public async Task<List<Receivable>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Receivables.AsNoTracking()
                .Include(x => x.PaymentReceipts)
                .Include(x => x.Invoice)
                .Include(x => x.Buyer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => 
                    (x.Invoice != null && x.Invoice.FiscalSupportNumber != null && x.Invoice.FiscalSupportNumber.ToLower().Contains(searchTerm)) ||
                    (x.Buyer != null && x.Buyer.Name.ToLower().Contains(searchTerm))
                );
            }

            if (_httpContext != null)
            {
                await _httpContext.AddPaginationParamsToHeaders(queryable);
            }

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }
    }
}
