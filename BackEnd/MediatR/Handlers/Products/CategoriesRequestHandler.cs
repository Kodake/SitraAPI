using BackEnd.DTOs.Products.Categories;
using BackEnd.MediatR.Commands.Products.Categories;
using BackEnd.MediatR.Queries.Products.Categories;
using MediatR;

namespace BackEnd.MediatR.Handlers.Products
{
    public static class CategoriesRequestHandler
    {
        public static IServiceCollection AddCategoryServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateCategoryCommand, bool>, ToggleCommandHandler<ActivateCategoryCommand>>();
            services.AddTransient<IRequestHandler<DeactivateCategoryCommand, bool>, ToggleCommandHandler<DeactivateCategoryCommand>>();
            services.AddTransient<IRequestHandler<GetCategoryByIdQuery, CategoryDTO>, QueryHandler<GetCategoryByIdQuery, CategoryDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsCategoriesQuery, List<CategoryListDTO>>, QueryHandler<GetSelectsCategoriesQuery, List<CategoryListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllCategoriesQuery, List<CategoryListDTO>>, QueryHandler<GetAllCategoriesQuery, List<CategoryListDTO>>>();
            services.AddTransient<IRequestHandler<CreateCategoryCommand, CreateCategoryDTO>, CreateCommandHandler<Entities.Products.Category, CreateCategoryDTO>>();
            services.AddTransient<IRequestHandler<UpdateCategoryCommand, bool>, UpdateCommandHandler<Entities.Products.Category, CreateCategoryDTO>>();

            return services;
        }
    }
}
