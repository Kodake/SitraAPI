using AutoMapper;
using BackEnd.DTOs.Products.Combos;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Combos
{
    public class UpdateComboCommand : UpdateCommand<Item, CreateComboDTO>
    {
        public UpdateComboCommand(long id, CreateComboDTO dto) : base(id, dto) { }

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