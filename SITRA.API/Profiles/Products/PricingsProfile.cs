using AutoMapper;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Entities.Products;

namespace SITRA.API.Profiles.Products
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

