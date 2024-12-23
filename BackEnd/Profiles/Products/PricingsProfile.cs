using AutoMapper;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Entities.Products;

namespace BackEnd.Profiles.Products
{
    public class PricingsProfile : Profile
    {
        public PricingsProfile()
        {
            CreateMap<CreatePricingDTO, Pricing>();
            CreateMap<Pricing, PricingListDTO>();
            CreateMap<Pricing, PricingDTO>();
        }
    }
}
