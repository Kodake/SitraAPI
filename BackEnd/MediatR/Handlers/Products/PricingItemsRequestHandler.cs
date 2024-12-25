using BackEnd.DTOs.Products.PricingItems;
using BackEnd.MediatR.Commands.Products.PricingItems;
using BackEnd.MediatR.Queries.Products.PricingItems;
using MediatR;

namespace BackEnd.MediatR.Handlers.Products
{
    public static class PricingItemsRequestHandler
    {
        public static IServiceCollection AddPricingItemServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivatePricingItemCommand, bool>, ToggleCommandHandler<ActivatePricingItemCommand>>();
            services.AddTransient<IRequestHandler<CreatePricingItemCommand, CreatePricingItemDTO>, CreateCommandHandler<Entities.Products.PricingItem, CreatePricingItemDTO>>();
            services.AddTransient<IRequestHandler<DeletePricingItemCommand, bool>, DeleteCommandHandler<DeletePricingItemCommand>>();
            services.AddTransient<IRequestHandler<GetAllPricingItemsByIdQuery, List<PricingItemListDTO>>, QueryHandler<GetAllPricingItemsByIdQuery, List<PricingItemListDTO>>>();
            return services;
        }
    }
}