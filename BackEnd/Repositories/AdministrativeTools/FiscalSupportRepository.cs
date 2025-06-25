using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.AdministrativeTools
{
    public class FiscalSupportRepository : RepositoryBase<FiscalSupport>, IFiscalSupportRepository
    {
        private new readonly HttpContext httpContext;
        public FiscalSupportRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<FiscalSupport>> GetAllAsync(PaginationDTO paginationDTO)
        {
            var queryable = context.FiscalSupports
                .AsQueryable()
                .Include(p => p.Warehouse);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<FiscalSupport>> GetAll(StatusType status)
        {
            IQueryable<FiscalSupport> query = context.FiscalSupports.AsQueryable();

            switch (status)
            {
                case StatusType.Activated:
                    query = query.Where(u => u.IsActive);
                    break;
                case StatusType.Deactivated:
                    query = query.Where(u => !u.IsActive);
                    break;
                case StatusType.All:
                default:
                    break;
            }

            return await query.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<FiscalSupport?> GetByIdAsync(long? id)
        {
            return await context.FiscalSupports
                .Include(x => x.Warehouse)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<FiscalSupport>> GetByDocumentTypeAndWarehouseId(FiscalSupportType supportType, long warehouseId)
        {
            return await context.FiscalSupports
                .Include(x => x.Warehouse)
                .Where(x => x.FiscalSupportType == supportType && x.WarehouseId == warehouseId)
                .ToListAsync();
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.FiscalSupports.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}