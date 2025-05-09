using BackEnd.DTOs.Products.Brands;
using BackEnd.Mediator.Commands.Products.Brands;
using BackEnd.Mediator.Queries.Products.Brands;
using MediatR;

namespace BackEnd.Mediator.Handlers.Products
{
    public static class BrandsRequestHandler
    {
        public static IServiceCollection AddBrandServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateBrandCommand, bool>, ToggleCommandHandler<ActivateBrandCommand>>();
            services.AddTransient<IRequestHandler<DeactivateBrandCommand, bool>, ToggleCommandHandler<DeactivateBrandCommand>>();
            services.AddTransient<IRequestHandler<GetBrandByIdQuery, BrandDTO>, QueryHandler<GetBrandByIdQuery, BrandDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsBrandsQuery, List<BrandListDTO>>, QueryHandler<GetSelectsBrandsQuery, List<BrandListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllBrandsQuery, List<BrandListDTO>>, QueryHandler<GetAllBrandsQuery, List<BrandListDTO>>>();
            services.AddTransient<IRequestHandler<CreateBrandCommand, CreateBrandDTO>, CreateCommandHandler<Entities.Products.Brand, CreateBrandDTO>>();
            services.AddTransient<IRequestHandler<UpdateBrandCommand, bool>, UpdateCommandHandler<Entities.Products.Brand, CreateBrandDTO>>();

            return services;
        }
    }
}
