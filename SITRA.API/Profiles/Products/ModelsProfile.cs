using AutoMapper;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.Entities.Products;
namespace SITRA.API.Profiles.Products
{
    public class ModelsProfile : Profile
    {
        public ModelsProfile()
        {
            CreateMap<CreateModelDTO, Model>();
            CreateMap<Model, ModelListDTO>()
                .ForMember(des => des.BrandName, opt => opt.MapFrom(src =>
                    src.Brand.Name));
            CreateMap<Model, ModelDTO>();
        }
    }
}

