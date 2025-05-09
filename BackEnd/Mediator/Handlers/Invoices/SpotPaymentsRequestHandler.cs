using BackEnd.DTOs.Invoices.SpotPayments;
using BackEnd.Mediator.Queries.Invoices.SpotPayments;
using MediatR;

namespace BackEnd.Mediator.Handlers.Invoices
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
