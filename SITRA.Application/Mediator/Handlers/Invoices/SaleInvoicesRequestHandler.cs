using Mediator;
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
            services.AddTransient<IRequestHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>, GetAllSaleInvoicesQueryHandler>();

            // services.AddTransient<IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>, CommandHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<CreateNoteCommand, CreateNoteDTO>, CreateNoteCommandHandler>();
            services.AddTransient<IRequestHandler<AnnulNcfCommand, AnnulNcfDTO>, AnnulNcfCommandHandler>();
            services.AddTransient<IRequestHandler<CreateSaleInvoiceCommand, CreateSaleInvoiceDTO>, CreateSaleInvoiceCommandHandler>();
            services.AddTransient<IRequestHandler<CreateSaleInvoiceWithItemCommand, CreateSaleInvoiceWithItemDTO>, CreateSaleInvoiceWithItemCommandHandler>();
            services.AddTransient<IRequestHandler<GetSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, GetSaleInvoicePdfQueryHandler>();
            services.AddTransient<IRequestHandler<GetSaleInvoiceReceiptPdfQuery, (byte[] FileBytes, string FileName)?>, GetSaleInvoiceReceiptPdfQueryHandler>();

            services.AddTransient<IRequestHandler<GetSaleInvoicesBalanceQuery, BalanceSummaryDTO>, GetSaleInvoicesBalanceQueryHandler>();
            services.AddTransient<IRequestHandler<GetSaleInvoicesWithBalanceQuery, SaleInvoiceWithBalanceDTO>, GetSaleInvoicesWithBalanceQueryHandler>();
            services.AddTransient<IRequestHandler<DownloadSaleInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, DownloadSaleInvoicePdfQueryHandler>();

            return services;
        }
    }
}

