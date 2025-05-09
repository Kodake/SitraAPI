using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Mediator.Commands.Invoices.Quotations;
using BackEnd.Mediator.Queries.Invoices.Quotations;
using MediatR;

namespace BackEnd.Mediator.Handlers.Invoices
{
    public static class QuotationsRequestHandler
    {
        public static IServiceCollection AddQuotationsServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetQuotationByIdQuery, QuotationDTO>, QueryHandler<GetQuotationByIdQuery, QuotationDTO>>();
            services.AddTransient<IRequestHandler<GetAllQuotationsQuery, List<QuotationListDTO>>, QueryHandler<GetAllQuotationsQuery, List<QuotationListDTO>>>();
            services.AddTransient<IRequestHandler<CreateQuotationCommand, CreateQuotationDTO>, CreateCommandHandler<Entities.Invoices.Quotation, CreateQuotationDTO>>();
            services.AddTransient<IRequestHandler<UpdateQuotationCommand, bool>, UpdateCommandHandler<Entities.Invoices.Quotation, CreateQuotationDTO>>();
            services.AddTransient<IRequestHandler<ProcessQuotationCommand, bool>, UpdateCommandHandler<Entities.Invoices.Quotation, CreateSaleInvoiceDTO>>();
            services.AddTransient<IRequestHandler<GetQuotationPdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetQuotationPdfQuery, (byte[] FileBytes, string FileName)?>>();

            return services;
        }
    }
}
