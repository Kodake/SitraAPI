using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Wizard;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Wizard
{
    public class CreateWizardCommand : CreateCommand<Item, WizardDTO>
    {
        public CreateWizardCommand(WizardDTO dto) : base(dto) { }

        public override async Task<WizardDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var dto = Dto;

            // 1. Currency
            var currency = mapper.Map<Currency>(dto.Currency);
            await unitOfWork.CurrencyRepository.CreateAsync(currency);

            // 2. Tax
            var tax = mapper.Map<Tax>(dto.Tax);
            var taxSaved = await unitOfWork.TaxRepository.CreateAsync(tax);

            // 3. Warehouse
            var warehouse = mapper.Map<Warehouse>(dto.Warehouse);
            var warehouseSaved = await unitOfWork.WarehouseRepository.CreateAsync(warehouse);

            // 4. Product (Item)
            var product = mapper.Map<Item>(dto.Product);
            product.ItemType = ItemType.Product;

            // Handle the special -1 tax id
            if (dto.Product.TaxIds != null)
            {
                var existingTaxIds = dto.Product.TaxIds.Where(id => id != -1).ToList();
                product.ItemTaxes = existingTaxIds.Select(id => new ItemTaxes { TaxId = id }).ToList();

                if (dto.Product.TaxIds.Contains(-1))
                {
                    product.ItemTaxes.Add(new ItemTaxes { Tax = taxSaved });
                }
            }

            var productSaved = await unitOfWork.ProductRepository.CreateAsync(product);

            // 5. Pricing
            var pricing = await unitOfWork.PricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
            if (pricing == null)
            {
                pricing = new Pricing
                {
                    Name = Enum.GetName(PricingName.Principal)?.ToUpper() ?? "PRINCIPAL",
                    PricingType = PricingType.Value
                };
                pricing = await unitOfWork.PricingRepository.CreateAsync(pricing);
            }

            var pricingItem = new PricingItem
            {
                Item = productSaved,
                Pricing = pricing,
                InitialCost = dto.Product.InitialCost,
                BasePrice = dto.Product.BasePrice,
                IsPrincipal = true
            };

            await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);

            // 6. Stock
            var stock = new Stock
            {
                Item = productSaved,
                Warehouse = warehouseSaved,
                Qty = dto.Stock.Qty,
                MinQty = 0,
                MaxQty = 0
            };

            await unitOfWork.StockRepository.CreateAsync(stock);

            await unitOfWork.SaveChangesAsync();

            // Evict caches
            await outputCacheStore.EvictByTagAsync("products-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("warehouses-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("taxes-get", cancellationToken);
            await outputCacheStore.EvictByTagAsync("currencies-get", cancellationToken);

            return dto;
        }
    }
}
