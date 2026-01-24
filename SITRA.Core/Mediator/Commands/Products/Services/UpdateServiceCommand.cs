using AutoMapper;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Services
{
    public class UpdateServiceCommand : UpdateCommand<Item, CreateServiceDTO>
    {
        public UpdateServiceCommand(long id, CreateServiceDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var serviceDB = await unitOfWork.ServiceRepository.GetByIdAsync(Id);
            if (serviceDB is null)
            {
                return false;
            }

            mapper.Map(Dto, serviceDB);

            serviceDB.ItemTaxes = Dto.TaxIds.Select(taxId => new ItemTaxes
            {
                TaxId = taxId,
                ItemId = serviceDB.Id
            }).ToList();

            serviceDB.ItemCategory = Dto.CategoryId.HasValue ? new ItemCategory { CategoryId = Dto.CategoryId.Value, ItemId = serviceDB.Id } : null;
            serviceDB.ItemSubCategory = Dto.SubCategoryId.HasValue ? new ItemSubCategory { SubCategoryId = Dto.SubCategoryId.Value, ItemId = serviceDB.Id } : null;
            serviceDB.ItemUnit = Dto.UnitId.HasValue ? new ItemUnit { UnitId = Dto.UnitId.Value, ItemId = serviceDB.Id } : null;

            var principalPricingItem = serviceDB.PricingItems.FirstOrDefault(x => x.IsPrincipal);
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

            unitOfWork.ServiceRepository.Update(serviceDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("services-get", default);

            return true;
        }
    }
}
