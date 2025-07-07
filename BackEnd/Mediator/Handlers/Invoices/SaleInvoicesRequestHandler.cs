using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Mediator.Commands.Invoices.SaleInvoices;
using BackEnd.Mediator.Queries.Invoices.SaleInvoices;
using MediatR;

namespace BackEnd.Mediator.Handlers.Invoices
{
    public static class SaleInvoicesRequestHandler
    {
        public static IServiceCollection AddSaleInvoicesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDTO>, QueryHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>, QueryHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllSaleInvoicesByDateRangeAndBillerQuery, List<SaleInvoiceListDTO>>, QueryHandler<GetAllSaleInvoicesByDateRangeAndBillerQuery, List<SaleInvoiceListDTO>>>();
            services.AddTransient<IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>, CreateCommandHandler<Entities.Invoices.Invoice, CreateSaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)?>>();
            services.AddTransient<IRequestHandler<GetSaleInvoiceReceiptPdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetSaleInvoiceReceiptPdfQuery, (byte[] FileBytes, string FileName)?>>();
            services.AddTransient<IRequestHandler<GetSaleInvoiceReportPdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetSaleInvoiceReportPdfQuery, (byte[] FileBytes, string FileName)?>>();

            return services;
        }
    }
}
