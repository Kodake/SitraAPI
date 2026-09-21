using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Products
{
    public class CreateProductCommand : CreateCommand<Item, CreateProductDTO>
    {
        private static readonly string container = "products";
        public CreateProductCommand(CreateProductDTO dto) : base(dto) { }

        public override async Task<CreateProductDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var product = mapper.Map<Item>(Dto);
            product.ItemType = ItemType.Product;

            var productSaved = await unitOfWork.ProductRepository.CreateAsync(product);

            if (Dto.ImageUrl is not null)
            {
                string rnc = await SITRA.Core.Helpers.TenantFiscalSupportHelper.GetCurrentTenantRncAsync(
                    unitOfWork, unitOfWork.HttpContextAccessor, cancellationToken);
                var url = await unitOfWork.DgiiStorageService.SaveItemPictureAsync(Dto.ImageUrl, rnc, productSaved.Id);
                productSaved.ImageUrl = url;
            }

            var pricing = await unitOfWork.PricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
            if (pricing == null)
            {
                pricing = new Pricing
                {
                    Name = Enum.GetName(PricingName.Principal)?.ToUpper() ?? "PRINCIPAL",
                    PricingType = PricingType.Value,
                    IsDefault = true
                };
                pricing = await unitOfWork.PricingRepository.CreateAsync(pricing);
            }

            var pricingItem = new PricingItem
            {
                Item = productSaved,
                Pricing = pricing,
                InitialCost = Dto.InitialCost,
                BasePrice = Dto.BasePrice,
                IsPrincipal = true
            };

            await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);

            if (Dto.WarehouseId is not null)
            {
                // Validation ensures warehouse exists
                var warehouse = await unitOfWork.WarehouseRepository.GetByIdAsync(Dto.WarehouseId.Value);

                var stock = new Stock
                {
                    Item = productSaved,
                    Qty = 0,
                    MinQty = 0,
                    MaxQty = 0,
                    Warehouse = warehouse!
                };

                await unitOfWork.StockRepository.CreateAsync(stock);
            }

            await unitOfWork.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("products-get", default);

            return Dto;
        }
    }
}
