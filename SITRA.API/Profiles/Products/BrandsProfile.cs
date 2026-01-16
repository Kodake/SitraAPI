using AutoMapper;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Entities.Products;

namespace SITRA.API.Profiles.Products
{
    public class BrandsProfile : Profile
    {
        public BrandsProfile()
        {
            CreateMap<CreateBrandDTO, Brand>();
            CreateMap<Brand, BrandListDTO>();
            CreateMap<Brand, BrandDTO>();
        }
    }
}

