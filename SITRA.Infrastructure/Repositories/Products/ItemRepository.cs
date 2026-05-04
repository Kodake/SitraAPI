using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Products;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories.Generics;

namespace SITRA.Infrastructure.Repositories.Products
{
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        { }

        public async Task<bool> Exists(Guid id, ItemType itemType, CancellationToken cancellationToken = default)
        {
            return await context.Items
                .AnyAsync(i => i.Id == id && i.ItemType == itemType, cancellationToken);
        }

        public async Task<List<Item>> GetByPricingIdAndWarehouseId(Guid pricingId, Guid warehouseId, string? fiscalSupportType = null, CancellationToken cancellationToken = default)
        {
            var query = context.Items
                .Include(x => x.Stocks)
                .Include(x => x.ItemTaxes)
                    .ThenInclude(it => it.Tax)
                .Include(x => x.PricingItems)
                    .ThenInclude(pi => pi.Pricing)
                .Include(x => x.ComboItems)
                    .ThenInclude(ci => ci.ChildItem)
                        .ThenInclude(child => child.Stocks)
                .Include(x => x.ComboItems)
                    .ThenInclude(ci => ci.ChildItem)
                        .ThenInclude(child => child.PricingItems)
                .Where(x =>
                    x.PricingItems.Any(pi => pi.PricingId == pricingId) &&
                    (
                        x.ItemType == ItemType.Service ||
                        x.Stocks.Any(s => s.WarehouseId == warehouseId)
                    )
                    || (
                        x.ItemType == ItemType.Combo &&
                        x.ComboItems.Any(ci =>
                            ci.ChildItem.PricingItems.Any(pi => pi.PricingId == pricingId) &&
                            ci.ChildItem.Stocks.Any(s => s.WarehouseId == warehouseId)
                        )
                    )
                );

                if (fiscalSupportType == "43" || fiscalSupportType == "44")
                {
                    // E43 and E44: DGII only accepts Exento items in practice
                    query = query.Where(x =>
                        x.ItemTaxes.Any(it => it.Tax.TaxType == TaxType.EXENTO)
                    );
                }
                else if (fiscalSupportType == "41")
                {
                    // E41 (Comprobante de Compras): exclude Exento and No Facturable.
                    // - Goods: must have ITBIS tax configured
                    // - Services: must have ISR tax configured (DGII requires 10% ISR retention on services)
                    query = query.Where(x =>
                        !x.ItemTaxes.Any(it =>
                            it.Tax.TaxType == TaxType.EXENTO ||
                            it.Tax.TaxType == TaxType.NO_FACTURABLE
                        ) &&
                        (
                            x.ItemType != ItemType.Service ||
                            x.ItemTaxes.Any(it => it.Tax.TaxType == TaxType.ISR)
                        )
                    );
                }
                else
                {
                    // All other types (31, 32, etc.): exclude Exento and No Facturable
                    query = query.Where(x =>
                        !x.ItemTaxes.Any(it =>
                            it.Tax.TaxType == TaxType.EXENTO ||
                            it.Tax.TaxType == TaxType.NO_FACTURABLE
                        )
                    );
                }

            return await query.ToListAsync(cancellationToken);
        }
    }
}





