using AutoMapper;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Entities.Products;
namespace SITRA.API.Profiles.Products
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

