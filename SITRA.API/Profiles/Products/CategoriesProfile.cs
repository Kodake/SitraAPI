using AutoMapper;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Entities.Products;
namespace SITRA.API.Profiles.Products
{
    public class CategoriesProfile : Profile
    {
        public CategoriesProfile()
        {
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<Category, CategoryListDTO>();
            CreateMap<Category, CategoryDTO>();
        }
    }
}

