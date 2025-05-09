using BackEnd.DTOs.Invoices.SpotPayables;
using BackEnd.Mediator.Queries.Invoices.SpotPayables;
using MediatR;

namespace BackEnd.Mediator.Handlers.Invoices
{
    public static class SpotPayablesRequestHandler
    {
        public static IServiceCollection AddSpotPayablesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSpotPayableByIdQuery, SpotPayableDTO>, QueryHandler<GetSpotPayableByIdQuery, SpotPayableDTO>>();
            services.AddTransient<IRequestHandler<GetAllSpotPayablesQuery, List<SpotPayableListDTO>>, QueryHandler<GetAllSpotPayablesQuery, List<SpotPayableListDTO>>>();
            return services;
        }
    }
}
