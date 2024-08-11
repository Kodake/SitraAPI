using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public CategoryRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Category>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Categories.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAll(bool isDeleted)
        {
            return await context.Categories
                .Where(u => u.IsDeleted == isDeleted)
                .OrderBy(u => u.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Create(Category category)
        {
            context.Add(category);
            await context.SaveChangesAsync();
            return category.Id;
        }

        public async Task Update(Category category)
        {
            context.Update(category);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category != null)
            {
                category.IsDeleted = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category != null)
            {
                category.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Categories.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Categories.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
