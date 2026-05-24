using MediatR;
using SITRA.Core.DTOs.Invoices.LogInvoices;
using SITRA.Core.Mediator.Queries.Invoices.LogInvoices;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public static class LogInvoicesRequestHandler
    {
        public static IServiceCollection AddLogInvoicesInvoicesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetAllLogInvoicesQuery, List<LogInvoiceListDTO>>, QueryHandler<GetAllLogInvoicesQuery, List<LogInvoiceListDTO>>>();
            services.AddTransient<IRequestHandler<GetLogInvoiceXmlQuery, string>, QueryHandler<GetLogInvoiceXmlQuery, string>>();

            return services;
        }
    }
}
