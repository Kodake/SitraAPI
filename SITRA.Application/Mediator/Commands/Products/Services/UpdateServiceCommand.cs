using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Services
{
    public class UpdateServiceCommand : UpdateCommand<Item, CreateServiceDTO>
    {
        private static readonly string container = "services";

        public UpdateServiceCommand(Guid id, CreateServiceDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var serviceDB = await unitOfWork.ServiceRepository.GetByIdAsync(Id);
            if (serviceDB is null)
            {
                return false;
            }

            var currentImageUrl = serviceDB.ImageUrl;

            mapper.Map(Dto, serviceDB);

            serviceDB.ImageUrl = currentImageUrl;

            if (Dto.ImageUrl is not null)
            {
                if (!string.IsNullOrEmpty(currentImageUrl))
                {
                    await unitOfWork.DgiiStorageService.DeleteItemPictureAsync(currentImageUrl);
                }

                string rnc = await SITRA.Core.Helpers.TenantFiscalSupportHelper.GetCurrentTenantRncAsync(
                    unitOfWork, unitOfWork.HttpContextAccessor, cancellationToken);
                var newUrl = await unitOfWork.DgiiStorageService.SaveItemPictureAsync(
                    Dto.ImageUrl,
                    rnc,
                    serviceDB.Id
                );

                serviceDB.ImageUrl = newUrl;
            }

            serviceDB.ItemTaxes = Dto.TaxIds.Select(taxId => new ItemTaxes
            {
                TaxId = taxId,
                ItemId = serviceDB.Id
            }).ToList();

            serviceDB.CategoryId = Dto.CategoryId;
            serviceDB.SubCategoryId = Dto.SubCategoryId;

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
