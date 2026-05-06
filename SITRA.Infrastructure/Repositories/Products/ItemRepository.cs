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
                .Where(x => x.PricingItems.Any(pi => pi.PricingId == pricingId) &&
                           (x.ItemType == ItemType.Service || x.Stocks.Any(s => s.WarehouseId == warehouseId)));

            if (fiscalSupportType == "43" || fiscalSupportType == "44")
            {
                query = query.Where(x => x.ItemTaxes.Any(it => it.Tax != null && it.Tax.TaxType == TaxType.EXENTO));
            }
            else if (fiscalSupportType == "47")
            {
                // e47: solo servicios exentos para no-residentes
                // El mapper aplica ISR 27% por defecto aunque no esté configurado en el ítem
                query = query.Where(x =>
                    x.ItemType == ItemType.Service &&
                    x.ItemTaxes.Any(it => it.Tax != null && it.Tax.TaxType == TaxType.EXENTO)
                );
            }
            else if (fiscalSupportType == "46")
            {
                query = query.Where(x => x.ItemTaxes.Any(it => it.Tax != null && it.Tax.TaxType == TaxType.ITBIS && it.Tax.Amount == 0));
            }
            else if (fiscalSupportType == "41")
            {
                query = query.Where(x =>
                    !x.ItemTaxes.Any(it => it.Tax != null && (
                        it.Tax.TaxType == TaxType.EXENTO ||
                        it.Tax.TaxType == TaxType.NO_FACTURABLE ||
                        (it.Tax.TaxType == TaxType.ITBIS && it.Tax.Amount == 0)
                    )) &&
                    (x.ItemType != ItemType.Service || x.ItemTaxes.Any(it => it.Tax != null && it.Tax.TaxType == TaxType.ISR))
                );
            }
            else if (fiscalSupportType != "45") 
            {
                query = query.Where(x => !x.ItemTaxes.Any(it => it.Tax != null && (it.Tax.TaxType == TaxType.EXENTO || it.Tax.TaxType == TaxType.NO_FACTURABLE)));
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}