using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Mediator.Commands.Products.Categories;
using SITRA.Core.Mediator.Queries.Products.Categories;
using MediatR;

namespace SITRA.Core.Mediator.Handlers.Products
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

