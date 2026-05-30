using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Infrastructure.Persistence;

namespace SITRA.Infrastructure.Services.Products
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> GetValidItemsForPricingAndWarehouseAsync(Guid pricingId, Guid warehouseId, string? fiscalSupportType = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Items
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
