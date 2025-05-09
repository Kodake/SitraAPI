using BackEnd.DTOs.Products.Items;
using BackEnd.Mediator.Queries.Products.Items;
using MediatR;

namespace BackEnd.Mediator.Handlers.Products
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
