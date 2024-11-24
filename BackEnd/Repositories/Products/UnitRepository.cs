using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class UnitRepository : IUnitRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public UnitRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Unit>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Units.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Unit>> GetAll(StatusType status)
        {
            IQueryable<Unit> query = context.Units.AsQueryable();

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

        public async Task<Unit?> GetById(long id)
        {
            return await context.Units.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long> Create(Unit unit)
        {
            context.Add(unit);
            await context.SaveChangesAsync();
            return unit.Id;
        }

        public async Task Update(Unit unit)
        {
            context.Update(unit);
            await context.SaveChangesAsync();
        }

        public async Task Activate(long id)
        {
            var unit = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if (unit != null)
            {
                unit.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(long id)
        {
            var unit = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if (unit != null)
            {
                unit.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(long id)
        {
            return await context.Units.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(long id, string name)
        {
            return await context.Units.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
