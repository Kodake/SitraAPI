using MediatR;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Queries.Invoices.SaleInvoices;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public static class SaleInvoicesRequestHandler
    {
        public static IServiceCollection AddSaleInvoicesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDTO>, QueryHandler<GetSaleInvoiceByIdQuery, SaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>, QueryHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllSaleInvoicesByDateRangeAndBillerQuery, List<SaleInvoiceListDTO>>, QueryHandler<GetAllSaleInvoicesByDateRangeAndBillerQuery, List<SaleInvoiceListDTO>>>();
            // services.AddTransient<IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>, CommandHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>, CreateSaleInvoiceCommandHandler>();
            services.AddTransient<IRequestHandler<GetSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)?>>();
            services.AddTransient<IRequestHandler<GetSaleInvoiceReceiptPdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetSaleInvoiceReceiptPdfQuery, (byte[] FileBytes, string FileName)?>>();
            services.AddTransient<IRequestHandler<GetSaleInvoiceReportPdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetSaleInvoiceReportPdfQuery, (byte[] FileBytes, string FileName)?>>();
            services.AddTransient<IRequestHandler<GetSaleInvoicesBalanceQuery, BalanceSummaryDTO>, GetSaleInvoicesBalanceQueryHandler>();

            return services;
        }
    }
}

