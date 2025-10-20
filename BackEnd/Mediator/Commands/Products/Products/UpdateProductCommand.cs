using AutoMapper;
using BackEnd.DTOs.Products.Products;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Products
{
    public class UpdateProductCommand : UpdateCommand<Item, CreateProductDTO>
    {
        private static readonly string container = "products";

        public UpdateProductCommand(long id, CreateProductDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var productDB = await unitOfWork.ProductRepository.GetByIdAsync(Id);
            if (productDB is null)
                return false;

            var currentImageUrl = productDB.ImageUrl;

            mapper.Map(Dto, productDB);

            productDB.ImageUrl = currentImageUrl;

            if (Dto.ImageUrl is not null)
            {
                var newUrl = await unitOfWork.FileStorageService.UpdateAsync(
                    currentImageUrl,
                    container,
                    Dto.ImageUrl
                );

                productDB.ImageUrl = newUrl;
            }

            productDB.ItemTaxes = Dto.TaxIds.Select(taxId => new ItemTaxes
            {
                TaxId = taxId,
                ItemId = productDB.Id
            }).ToList();

            var principalPricingItem = productDB.PricingItems.FirstOrDefault(x => x.IsPrincipal);
            if (principalPricingItem != null)
            {
                var pricingItem = await unitOfWork.PricingItemRepository
                    .GetPricingItemByItemIdAndPricingId(Id, principalPricingItem.PricingId);

                if (pricingItem != null)
                {
                    pricingItem.InitialCost = Dto.InitialCost;
                    pricingItem.BasePrice = Dto.BasePrice;
                    unitOfWork.PricingItemRepository.Update(pricingItem);
                }
            }

            unitOfWork.ProductRepository.Update(productDB);
            await unitOfWork.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("products-get", default);

            return true;
        }
    }
}
