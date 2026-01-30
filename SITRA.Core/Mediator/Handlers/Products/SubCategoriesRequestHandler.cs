using MediatR;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Mediator.Commands.Products.SubCategories;
using SITRA.Core.Mediator.Queries.Products.SubCategories;

namespace SITRA.Core.Mediator.Handlers.Products
{
    public static class SubCategoriesRequestHandler
    {
        public static IServiceCollection AddSubCategoryServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateSubCategoryCommand, bool>, ToggleCommandHandler<ActivateSubCategoryCommand>>();
            services.AddTransient<IRequestHandler<DeactivateSubCategoryCommand, bool>, ToggleCommandHandler<DeactivateSubCategoryCommand>>();
            services.AddTransient<IRequestHandler<GetSubCategoryByIdQuery, SubCategoryDTO>, QueryHandler<GetSubCategoryByIdQuery, SubCategoryDTO>>();
            services.AddTransient<IRequestHandler<GetSubCategoriesByCategoryIdQuery, List<SubCategoryListDTO>>, QueryHandler<GetSubCategoriesByCategoryIdQuery, List<SubCategoryListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllSubCategoriesQuery, List<SubCategoryListDTO>>, QueryHandler<GetAllSubCategoriesQuery, List<SubCategoryListDTO>>>();
            services.AddTransient<IRequestHandler<CreateSubCategoryCommand, CreateSubCategoryDTO>, CreateCommandHandler<Entities.Products.SubCategory, CreateSubCategoryDTO>>();
            services.AddTransient<IRequestHandler<UpdateSubCategoryCommand, bool>, UpdateCommandHandler<Entities.Products.SubCategory, CreateSubCategoryDTO>>();

            return services;
        }
    }
}

