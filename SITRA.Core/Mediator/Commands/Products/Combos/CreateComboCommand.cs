using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Combos
{
    public class CreateComboCommand : CreateCommand<Item, CreateComboDTO>
    {
        public CreateComboCommand(CreateComboDTO dto) : base(dto) { }

        public override async Task<CreateComboDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var combo = mapper.Map<Item>(Dto);
            combo.ItemType = ItemType.Combo;

            var comboSaved = await unitOfWork.ComboRepository.CreateAsync(combo);

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
                Item = comboSaved,
                Pricing = pricing,
                InitialCost = Dto.InitialCost,
                BasePrice = Dto.BasePrice,
                IsPrincipal = true
            };

            await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);

            await unitOfWork.SaveChangesAsync();

            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("combos-get", default);

            return Dto;
        }
    }
}
