using BackEnd.DTOs.Products.Brands;
using BackEnd.Entities.Products;
using BackEnd.MediatR.Commands.Brands;
using BackEnd.MediatR.Handlers;
using BackEnd.MediatR.Queries.Brands;
using MediatR;

namespace BackEnd.MediatR
{
    public static class MediatRDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandler(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRDependencyHandler).Assembly));

            services.AddTransient<IRequestHandler<ActivateBrandCommand, bool>, ToggleCommandHandler<ActivateBrandCommand>>();
            services.AddTransient<IRequestHandler<DeactivateBrandCommand, bool>, ToggleCommandHandler<DeactivateBrandCommand>>();
            services.AddTransient<IRequestHandler<GetBrandByIdQuery, BrandDTO>, QueryHandler<GetBrandByIdQuery, BrandDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsBrandsQuery, List<BrandListDTO>>, QueryHandler<GetSelectsBrandsQuery, List<BrandListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllBrandsQuery, List<BrandListDTO>>, QueryHandler<GetAllBrandsQuery, List<BrandListDTO>>>();
            services.AddTransient<IRequestHandler<CreateBrandCommand, CreateBrandDTO>, CreateCommandHandler<Brand, CreateBrandDTO>>();
            services.AddTransient<IRequestHandler<UpdateBrandCommand, bool>, UpdateCommandHandler<Brand, CreateBrandDTO>>();

            return services;
        }
    }
}
