using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Services
{
    public class CreateServiceCommand : CreateCommand<Item, CreateServiceDTO>
    {
        public CreateServiceCommand(CreateServiceDTO dto) : base(dto) { }

        public override async Task<CreateServiceDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var service = mapper.Map<Item>(Dto);
            service.ItemType = ItemType.Service;

            var serviceSaved = await unitOfWork.ServiceRepository.CreateAsync(service);

            if (Dto.ImageUrl is not null)
            {
                string rnc = await SITRA.Core.Helpers.TenantFiscalSupportHelper.GetCurrentTenantRncAsync(
                    unitOfWork, unitOfWork.HttpContextAccessor, cancellationToken);
                var url = await unitOfWork.DgiiStorageService.SaveItemPictureAsync(Dto.ImageUrl, rnc, serviceSaved.Id);
                serviceSaved.ImageUrl = url;
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
                Item = serviceSaved,
                Pricing = pricing,
                InitialCost = Dto.InitialCost,
                BasePrice = Dto.BasePrice,
                IsPrincipal = true
            };

            await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
            await outputCacheStore.EvictByTagAsync("pricings-get", default);
            await outputCacheStore.EvictByTagAsync("services-get", default);

            return Dto;
        }
    }
}
