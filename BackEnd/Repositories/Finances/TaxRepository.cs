using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Finances;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Finances
{
    public class TaxRepository : ITaxRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public TaxRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Tax>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Taxes.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Tax>> GetAll(StatusType status)
        {
            IQueryable<Tax> query = context.Taxes.AsNoTracking();

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

        public async Task<Tax?> GetById(int id)
        {
            return await context.Taxes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Tax tax)
        {
            context.Add(tax);
            await context.SaveChangesAsync();
            return tax.Id;
        }

        public async Task Update(Tax tax)
        {
            context.Update(tax);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var tax = await context.Taxes.FirstOrDefaultAsync(x => x.Id == id);
            if (tax != null)
            {
                tax.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var tax = await context.Taxes.FirstOrDefaultAsync(x => x.Id == id);
            if (tax != null)
            {
                tax.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Taxes.AnyAsync(x => x.Id == id);
        }
    }
}
