using Mediator;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayments;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public static class SpotPaymentsRequestHandler
    {
        public static IServiceCollection AddSpotPaymentsServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSpotPaymentByIdQuery, SpotPaymentDTO>, QueryHandler<GetSpotPaymentByIdQuery, SpotPaymentDTO>>();
            services.AddTransient<IRequestHandler<GetAllSpotPaymentsQuery, List<SpotPaymentListDTO>>, GetAllSpotPaymentsQueryHandler>();
            return services;
        }
    }
}

