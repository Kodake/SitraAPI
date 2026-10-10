using AutoMapper;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.Entities.Products;

namespace SITRA.API.Profiles.Products
{
    public class PricingItemsProfile : Profile
    {
        public PricingItemsProfile()
        {
            CreateMap<CreatePricingItemDTO, PricingItem>();
            CreateMap<PricingItem, PricingItemListDTO>()
                .ForMember(des => des.Percentage, opt => opt.MapFrom(src =>
                    src.Pricing.Percentage));
            CreateMap<PricingItem, PricingItemDTO>();
        }
    }
}

