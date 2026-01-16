using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayments;
using MediatR;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public static class SpotPaymentsRequestHandler
    {
        public static IServiceCollection AddSpotPaymentsServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSpotPaymentByIdQuery, SpotPaymentDTO>, QueryHandler<GetSpotPaymentByIdQuery, SpotPaymentDTO>>();
            services.AddTransient<IRequestHandler<GetAllSpotPaymentsQuery, List<SpotPaymentListDTO>>, QueryHandler<GetAllSpotPaymentsQuery, List<SpotPaymentListDTO>>>();
            return services;
        }
    }
}

