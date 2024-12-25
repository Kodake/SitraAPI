using BackEnd.DTOs.Products.Pricings;
using BackEnd.MediatR.Commands.Products.Pricings;
using BackEnd.MediatR.Queries.Products.Pricings;
using MediatR;

namespace BackEnd.MediatR.Handlers.Products
{
    public static class PricingRequestHandler
    {
        public static IServiceCollection AddPricingServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetPricingByIdQuery, PricingDTO>, QueryHandler<GetPricingByIdQuery, PricingDTO>>();
            services.AddTransient<IRequestHandler<GetUnregisteredPricingsForItemQuery, IEnumerable<PricingListDTO>>, QueryHandler<GetUnregisteredPricingsForItemQuery, IEnumerable<PricingListDTO>>>();
            services.AddTransient<IRequestHandler<GetSelectsPricingsQuery, List<PricingListDTO>>, QueryHandler<GetSelectsPricingsQuery, List<PricingListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllPricingsQuery, List<PricingListDTO>>, QueryHandler<GetAllPricingsQuery, List<PricingListDTO>>>();
            services.AddTransient<IRequestHandler<CreatePricingCommand, CreatePricingDTO>, CreateCommandHandler<Entities.Products.Pricing, CreatePricingDTO>>();
            services.AddTransient<IRequestHandler<UpdatePricingCommand, bool>, UpdateCommandHandler<Entities.Products.Pricing, CreatePricingDTO>>();

            return services;
        }
    }
}
