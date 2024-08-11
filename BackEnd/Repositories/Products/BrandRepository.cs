using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
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

        public async Task<List<Brand>> GetAll(bool isDeleted)
        {
            return await context.Brands
                .Where(u => u.IsDeleted == isDeleted)
                .OrderBy(u => u.Name)
                .AsNoTracking()
                .ToListAsync();
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
                brand.IsDeleted = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var brand = await context.Brands.FirstOrDefaultAsync(x => x.Id == id);
            if (brand != null)
            {
                brand.IsDeleted = true;
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
