using AutoMapper;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.Entities.Products;

namespace BackEnd.Profiles.Products
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
