using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.AdministrativeTools;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.AdministrativeTools
{
    public class FiscalSupportRepository : RepositoryBase<FiscalSupport>, IFiscalSupportRepository
    {
        private new readonly HttpContext httpContext;
        public FiscalSupportRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<FiscalSupport>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.FiscalSupports
                .AsQueryable();

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }

        public async new Task<List<FiscalSupport>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.FiscalSupports.AsNoTracking()
                .AsQueryable();

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync(cancellationToken);
        }



        public async Task<FiscalSupport?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
        {
            return await context.FiscalSupports
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<FiscalSupport?> GetByDocumentTypeAsync(
            EcfType supportType,
            CancellationToken cancellationToken = default)
        {
            return await context.FiscalSupports
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.FiscalSupportType == supportType, cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.FiscalSupports.AnyAsync(x => x.Id != id && x.Name == name, cancellationToken);
        }

        public async Task<FiscalSupport?> GetByTypeAsync(EcfType fiscalSupportType, CancellationToken cancellationToken = default)
        {
            return await context.FiscalSupports
                .FirstOrDefaultAsync(x => x.FiscalSupportType == fiscalSupportType, cancellationToken);
        }

        public async Task<bool> SetDefault(Guid id, CancellationToken cancellationToken = default)
        {
            var fiscalSupport = await context.FiscalSupports.FindAsync(new object[] { id }, cancellationToken);
            if (fiscalSupport == null) return false;

            var defaults = await context.FiscalSupports
                .Where(x => x.IsDefault && x.Id != id)
                .ToListAsync(cancellationToken);

            foreach (var f in defaults)
                f.IsDefault = false;

            fiscalSupport.IsDefault = true;
            return true;
        }
    }
}





