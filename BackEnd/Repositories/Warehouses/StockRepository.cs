using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.Entities.Warehouses;
using BackEnd.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.Warehouses
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public StockRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Stock>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = context.Stocks.AsQueryable();
            await httpContext.AddPaginationParamsToHeaders(queryable);
            return await queryable
                .OrderBy(x => x.Product.Name)
                .Paginate(paginationDTO)
                .ToListAsync();
        }

        public async Task<Stock?> GetById(int productId, int warehouseId)
        {
            return await context.Stocks.FirstOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }

        public async Task<bool> Create(Stock stock)
        {
            context.Add(stock);
            int changes = await context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task Update(Stock stock)
        {
            context.Update(stock);
            await context.SaveChangesAsync();
        }
    }
}
