using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Mediator.Commands.Products.SubCategories;
using BackEnd.Mediator.Queries.Products.SubCategories;
using MediatR;

namespace BackEnd.Mediator.Handlers.Products
{
    public static class SubCategoriesRequestHandler
    {
        public static IServiceCollection AddSubCategoryServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateSubCategoryCommand, bool>, ToggleCommandHandler<ActivateSubCategoryCommand>>();
            services.AddTransient<IRequestHandler<DeactivateSubCategoryCommand, bool>, ToggleCommandHandler<DeactivateSubCategoryCommand>>();
            services.AddTransient<IRequestHandler<GetSubCategoryByIdQuery, SubCategoryDTO>, QueryHandler<GetSubCategoryByIdQuery, SubCategoryDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsSubCategoriesQuery, List<SubCategoryListDTO>>, QueryHandler<GetSelectsSubCategoriesQuery, List<SubCategoryListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllSubCategoriesQuery, List<SubCategoryListDTO>>, QueryHandler<GetAllSubCategoriesQuery, List<SubCategoryListDTO>>>();
            services.AddTransient<IRequestHandler<CreateSubCategoryCommand, CreateSubCategoryDTO>, CreateCommandHandler<Entities.Products.SubCategory, CreateSubCategoryDTO>>();
            services.AddTransient<IRequestHandler<UpdateSubCategoryCommand, bool>, UpdateCommandHandler<Entities.Products.SubCategory, CreateSubCategoryDTO>>();

            return services;
        }
    }
}
