using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class UnitRepository : IUnitRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public UnitRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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

        public async Task<Unit?> GetById(int id)
        {
            return await context.Units.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Unit unit)
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

        public async Task Activate(int id)
        {
            var brand = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if (brand != null)
            {
                brand.IsDeleted = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var brand = await context.Units.FirstOrDefaultAsync(x => x.Id == id);
            if (brand != null)
            {
                brand.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Units.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Units.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
