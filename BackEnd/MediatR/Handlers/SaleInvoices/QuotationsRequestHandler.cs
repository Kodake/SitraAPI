using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.MediatR.Commands.Invoices.Quotations;
using BackEnd.MediatR.Queries.Invoices.Quotations;
using MediatR;

namespace BackEnd.MediatR.Handlers.Invoices
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

            return services;
        }
    }
}
