using AutoMapper;
using BackEnd.DTOs.Products.Categories;
using BackEnd.Entities.Products;
namespace BackEnd.Profiles.Products
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
