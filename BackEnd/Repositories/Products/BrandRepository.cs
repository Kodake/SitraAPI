using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Finances;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public BrandRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Brand>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Brands.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Brand>> GetAll(StatusType status)
        {
            IQueryable<Brand> query = context.Brands.AsNoTracking();

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

        public async Task<Brand?> GetById(int id)
        {
            return await context.Brands.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Brand brand)
        {
            context.Add(brand);
            await context.SaveChangesAsync();
            return brand.Id;
        }

        public async Task Update(Brand brand)
        {
            context.Update(brand);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var brand = await context.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand != null)
            {
                brand.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var brand = await context.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand != null)
            {
                brand.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Brands.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Brands.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
