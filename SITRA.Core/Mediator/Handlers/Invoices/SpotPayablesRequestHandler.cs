using Mediator;
using SITRA.Core.DTOs.Invoices.SpotPayables;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayables;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public static class SpotPayablesRequestHandler
    {
        public static IServiceCollection AddSpotPayablesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSpotPayableByIdQuery, SpotPayableDTO>, QueryHandler<GetSpotPayableByIdQuery, SpotPayableDTO>>();
            services.AddTransient<IRequestHandler<GetAllSpotPayablesQuery, List<SpotPayableListDTO>>, GetAllSpotPayablesQueryHandler>();
            return services;
        }
    }
}

