using MediatR;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Mediator.Commands.Products.Brands;
using SITRA.Core.Mediator.Queries.Products.Brands;

namespace SITRA.Core.Mediator.Handlers.Products
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

