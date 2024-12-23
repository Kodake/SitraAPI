using AutoMapper;
using BackEnd.DTOs.Products.Brands;
using BackEnd.Entities.Products;

namespace BackEnd.Profiles.Products
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
