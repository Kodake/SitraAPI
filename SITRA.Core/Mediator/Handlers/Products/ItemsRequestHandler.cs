using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.Mediator.Queries.Products.Items;
using MediatR;

namespace SITRA.Core.Mediator.Handlers.Products
{
    public static class ItemsRequestHandler
    {
        public static IServiceCollection AddItemsServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetItemsByPricingIdAndWarehouseIdQuery, List<ItemListDTO>>, QueryHandler<GetItemsByPricingIdAndWarehouseIdQuery, List<ItemListDTO>>>();
            return services;
        }
    }
}

