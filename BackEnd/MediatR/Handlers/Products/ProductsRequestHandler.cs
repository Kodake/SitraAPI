using BackEnd.DTOs.Products.Products;
using BackEnd.MediatR.Commands.Products.Products;
using BackEnd.MediatR.Queries.Products.Products;
using MediatR;

namespace BackEnd.MediatR.Handlers.Products
{
    public static class ProductsRequestHandler
    {
        public static IServiceCollection AddProductServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateProductCommand, bool>, ToggleCommandHandler<ActivateProductCommand>>();
            services.AddTransient<IRequestHandler<DeactivateProductCommand, bool>, ToggleCommandHandler<DeactivateProductCommand>>();
            services.AddTransient<IRequestHandler<GetProductByIdQuery, ProductDTO>, QueryHandler<GetProductByIdQuery, ProductDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsProductsQuery, List<ProductListDTO>>, QueryHandler<GetSelectsProductsQuery, List<ProductListDTO>>>();
            services.AddTransient<IRequestHandler<GetStockProductsQuery, List<ProductListDTO>>, QueryHandler<GetStockProductsQuery, List<ProductListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllProductsQuery, List<ProductListDTO>>, QueryHandler<GetAllProductsQuery, List<ProductListDTO>>>();
            services.AddTransient<IRequestHandler<CreateProductCommand, CreateProductDTO>, CreateCommandHandler<Entities.Products.Item, CreateProductDTO>>();
            services.AddTransient<IRequestHandler<UpdateProductCommand, bool>, UpdateCommandHandler<Entities.Products.Item, CreateProductDTO>>();
            services.AddTransient<IRequestHandler<UploadCsvProductCommand, bool>, UploadCsvProductCommand.Handler>();

            return services;
        }
    }
}
