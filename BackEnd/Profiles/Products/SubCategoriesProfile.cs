using AutoMapper;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Entities.Products;
namespace BackEnd.Profiles.Products
{
    public class SubCategoriesProfile : Profile
    {
        public SubCategoriesProfile()
        {
            CreateMap<CreateSubCategoryDTO, SubCategory>();
            CreateMap<SubCategory, SubCategoryListDTO>()
                .ForMember(des => des.CategoryName, opt => opt.MapFrom(src =>
                    src.Category.Name));
            CreateMap<SubCategory, SubCategoryDTO>();
        }
    }
}
