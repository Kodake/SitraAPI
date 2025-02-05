using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.MediatR.Commands.Invoices.SaleInvoices;
using BackEnd.MediatR.Queries.Invoices.SaleInvoices;
using MediatR;

namespace BackEnd.MediatR.Handlers.Invoices
{
    public static class SaleInvoicesRequestHandler
    {
        public static IServiceCollection AddSaleInvoicesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDTO>, QueryHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>, QueryHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>>();
            services.AddTransient<IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>, CreateCommandHandler<Entities.Invoices.Invoice, CreateSaleInvoiceDTO>>();

            return services;
        }
    }
}
