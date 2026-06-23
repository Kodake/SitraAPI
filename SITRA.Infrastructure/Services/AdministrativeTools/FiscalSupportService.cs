using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Infrastructure.Persistence;
using System.Linq;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.AdministrativeTools
{
    public class FiscalSupportService : IFiscalSupportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly HttpContext? _httpContext;

        public FiscalSupportService(
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<FiscalSupport>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<FiscalSupport, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var items = await _unitOfWork.FiscalSupportRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }

        public async Task<List<FiscalSupport>> GetPaginatedAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var owner = await _context.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.IsOwner, cancellationToken);
            var isElectronic = owner?.IsElectronicTransmitter ?? false;

            var queryable = _context.FiscalSupports.AsNoTracking()
                .Where(x => isElectronic ? (int)x.FiscalSupportType >= 31 : (int)x.FiscalSupportType < 31)
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationDTO.SearchTerm))
            {
                var searchTerm = paginationDTO.SearchTerm.ToLower();
                queryable = queryable.Where(x => x.Name.ToLower().Contains(searchTerm));
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
