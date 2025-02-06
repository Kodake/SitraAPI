using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.MediatR.Commands.Invoices.PurchaseInvoices;
using BackEnd.MediatR.Queries.Invoices.PurchaseInvoices;
using MediatR;

namespace BackEnd.MediatR.Handlers.Invoices
{
    public static class PurchaseInvoicesRequestHandler
    {
        public static IServiceCollection AddPurchaseInvoicesServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetPurchaseInvoiceByIdQuery, PurchaseInvoiceDTO>, QueryHandler<GetPurchaseInvoiceByIdQuery, PurchaseInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetAllPurchaseInvoicesQuery, List<PurchaseInvoiceListDTO>>, QueryHandler<GetAllPurchaseInvoicesQuery, List<PurchaseInvoiceListDTO>>>();
            services.AddTransient<IRequestHandler<CreatePurchaseInvoiceCommand, CreatePurchaseInvoiceDTO>, CreateCommandHandler<Entities.Invoices.Invoice, CreatePurchaseInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetPurchaseInvoicePdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetPurchaseInvoicePdfQuery, (byte[] FileBytes, string FileName)?>>();

            return services;
        }
    }
}
