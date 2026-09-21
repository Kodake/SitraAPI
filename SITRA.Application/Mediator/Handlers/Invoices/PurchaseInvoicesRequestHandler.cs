using Mediator;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices;

namespace SITRA.Core.Mediator.Handlers.Invoices
{
    public static class PurchaseInvoicesRequestHandler
    {
        public static IServiceCollection AddPurchaseInvoicesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetPurchaseInvoiceByIdQuery, PurchaseInvoiceDTO>, QueryHandler<GetPurchaseInvoiceByIdQuery, PurchaseInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetAllPurchaseInvoicesQuery, List<PurchaseInvoiceListDTO>>, GetAllPurchaseInvoicesQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllPurchaseInvoicesByDateRangeQuery, List<PurchaseInvoiceListDTO>>, GetAllPurchaseInvoicesByDateRangeQueryHandler>();
            // services.AddTransient<IRequestHandler<CreatePurchaseInvoiceCommand, CreatePurchaseInvoiceDTO>, CreateCommandHandler<Entities.Invoices.Invoice, CreatePurchaseInvoiceDTO>>();
            services.AddTransient<IRequestHandler<CreatePurchaseInvoiceCommand, CreatePurchaseInvoiceDTO>, CreatePurchaseInvoiceCommandHandler>();
            services.AddTransient<IRequestHandler<GetPurchaseInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, GetPurchaseInvoicePdfQueryHandler>();
            services.AddTransient<IRequestHandler<GetPurchaseInvoiceReceiptPdfQuery, (byte[] FileBytes, string FileName)?>, GetPurchaseInvoiceReceiptPdfQueryHandler>();
            services.AddTransient<IRequestHandler<GetPurchaseInvoiceReportPdfQuery, (byte[] FileBytes, string FileName)?>, GetPurchaseInvoiceReportPdfQueryHandler>();
            services.AddTransient<IRequestHandler<DownloadPurchaseInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, DownloadPurchaseInvoicePdfQueryHandler>();
            services.AddTransient<IRequestHandler<GetPurchaseInvoicesWithBalanceQuery, PurchaseInvoiceWithBalanceDTO>, GetPurchaseInvoicesWithBalanceQueryHandler>();

            return services;
        }
    }
}

