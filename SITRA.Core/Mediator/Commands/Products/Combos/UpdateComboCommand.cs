using AutoMapper;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Combos
{
    public class UpdateComboCommand : UpdateCommand<Item, CreateComboDTO>
    {
        public UpdateComboCommand(Guid id, CreateComboDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var comboDB = await unitOfWork.ComboRepository.GetByIdAsync(Id);
            if (comboDB is null)
            {
                return false;
            }

            mapper.Map(Dto, comboDB);

            comboDB.ItemTaxes = Dto.TaxIds.Select(taxId => new ItemTaxes
            {
                TaxId = taxId,
                ItemId = comboDB.Id
            }).ToList();

            comboDB.ComboItems = Dto.Items.Select(i => new ComboItem
            {
                ChildItemId = i.Id,
                Qty = i.Qty,
                ParentItemId = comboDB.Id
            }).ToList();

            comboDB.ItemBrand = Dto.BrandId.HasValue ? new ItemBrand { BrandId = Dto.BrandId.Value, ItemId = comboDB.Id } : null;
            comboDB.ItemModel = Dto.ModelId.HasValue ? new ItemModel { ModelId = Dto.ModelId.Value, ItemId = comboDB.Id } : null;
            comboDB.ItemCategory = Dto.CategoryId.HasValue ? new ItemCategory { CategoryId = Dto.CategoryId.Value, ItemId = comboDB.Id } : null;
            comboDB.ItemSubCategory = Dto.SubCategoryId.HasValue ? new ItemSubCategory { SubCategoryId = Dto.SubCategoryId.Value, ItemId = comboDB.Id } : null;
            comboDB.ItemUnit = Dto.UnitId.HasValue ? new ItemUnit { UnitId = Dto.UnitId.Value, ItemId = comboDB.Id } : null;

            var principalPricingItem = comboDB.PricingItems.FirstOrDefault(x => x.IsPrincipal);
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

            unitOfWork.ComboRepository.Update(comboDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("combos-get", default);

            return true;
        }
    }
}
