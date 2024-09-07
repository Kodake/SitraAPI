using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.AdministrativeTools
{
    public class FiscalSupportRepository : IFiscalSupportRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public FiscalSupportRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<FiscalSupport>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.FiscalSupports
                .AsQueryable()
                .AsNoTracking()
                .Include(p => p.Warehouse);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<FiscalSupport>> GetAll(StatusType status)
        {
            IQueryable<FiscalSupport> query = context.FiscalSupports.AsNoTracking();

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

        public async Task<FiscalSupport?> GetById(int id)
        {
            return await context.FiscalSupports.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(FiscalSupport fiscalSupport)
        {
            context.Add(fiscalSupport);
            await context.SaveChangesAsync();
            return fiscalSupport.Id;
        }

        public async Task Update(FiscalSupport fiscalSupport)
        {
            context.Update(fiscalSupport);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var fiscalSupport = await context.FiscalSupports.FirstOrDefaultAsync(x => x.Id == id);
            if (fiscalSupport != null)
            {
                fiscalSupport.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var fiscalSupport = await context.FiscalSupports.FirstOrDefaultAsync(x => x.Id == id);
            if (fiscalSupport != null)
            {
                fiscalSupport.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.FiscalSupports.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.FiscalSupports.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}