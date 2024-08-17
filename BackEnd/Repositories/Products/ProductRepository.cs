using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public ProductRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Product>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Products.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Name)
                .Paginate(paginationDTO)
                .Include(p => p.ProductTaxes)
                .ThenInclude(pt => pt.Tax)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAll(StatusType status)
        {
            IQueryable<Product> query = context.Products.AsNoTracking();

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

        public async Task<Product?> GetById(int id)
        {
            return await context.Products
                .Include(p => p.ProductTaxes)
                .ThenInclude(pt => pt.Tax)
                .Include(p => p.Unit)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Create(Product product)
        {
            context.Add(product);
            await context.SaveChangesAsync();
            return product.Id;
        }

        public async Task Update(Product product)
        {
            context.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                product.IsActive = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task Deactivate(int id)
        {
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                product.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Products.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Exists(int id, string name)
        {
            return await context.Products.AnyAsync(x => x.Id != id && x.Name == name);
        }
    }
}
