using AutoMapper;
using BackEnd.DTOs.Products.Models;
using BackEnd.Entities.Products;
namespace BackEnd.Profiles.Products
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
