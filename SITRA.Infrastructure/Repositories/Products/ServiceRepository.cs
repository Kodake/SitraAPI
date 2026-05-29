using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.DTOs;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Products
{
    public class ServiceRepository : RepositoryBase<Item>, IServiceRepository
    {
        private new readonly HttpContext httpContext;
        public ServiceRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async new Task<List<Item>> GetAllAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Items
                .Where(x => x.ItemType == ItemType.Service)
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.PricingItems)
                .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                .ThenInclude(pt => pt.Tax)
                .ToListAsync(cancellationToken);

            return items;
        }

        public async new Task<List<Item>> GetReadOnlyAsync(PaginationDTO paginationDTO, CancellationToken cancellationToken = default)
        {
            var queryable = context.Items.AsNoTracking()
                .Where(x => x.ItemType == ItemType.Service)
                .OrderBy(x => x.Name);

            await httpContext.AddPaginationParamsToHeaders(queryable);

            var items = await queryable
                .Paginate(paginationDTO)
                .Include(p => p.PricingItems)
                .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                .ThenInclude(pt => pt.Tax)
                .ToListAsync(cancellationToken);

            return items;
        }

        public async Task<List<Item>> GetAll(StatusType status, CancellationToken cancellationToken = default)
        {
            IQueryable<Item> query = context.Items
                .Where(u => u.ItemType == ItemType.Service);

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

            return await query.OrderBy(u => u.Id).ToListAsync(cancellationToken);
        }

        public async new Task<Item?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Items
                .Where(x => x.ItemType == ItemType.Service)
                .Include(p => p.ItemCategory)
                    .ThenInclude(ic => ic.Category)
                .Include(p => p.ItemSubCategory)
                    .ThenInclude(isc => isc.SubCategory)
                .Include(p => p.PricingItems)
                    .ThenInclude(p => p.Pricing)
                .Include(p => p.ItemTaxes)
                    .ThenInclude(pt => pt.Tax)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async new Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var service = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Service, cancellationToken);
            if (service != null)
            {
                service.IsActive = true;
            }
        }

        public async new Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var service = await context.Items.FirstOrDefaultAsync(x => x.Id == id && x.ItemType == ItemType.Service, cancellationToken);
            if (service != null)
            {
                service.IsActive = false;
            }
        }

        public async new Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Items.AnyAsync(x => x.Id == id && x.ItemType == ItemType.Service, cancellationToken);
        }

        public async Task<bool> Exists(Guid id, string name, CancellationToken cancellationToken = default)
        {
            return await context.Items.AnyAsync(x => x.Id != id && x.Name == name && x.ItemType == ItemType.Service, cancellationToken);
        }
    }
}




