using MediatR;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Mediator.Commands.Accounts.Receivables;
using SITRA.Core.Mediator.Queries.Accounts.Receivables;
using SITRA.Core.Mediator.Queries.Receivables;

namespace SITRA.Core.Mediator.Handlers.Accounts
{
    public static class ReceivablesRequestHandler
    {
        public static IServiceCollection AddReceivableServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<DeactivateReceivableCommand, bool>, ToggleCommandHandler<DeactivateReceivableCommand>>();
            services.AddTransient<IRequestHandler<GetAllReceivablesQuery, List<ReceivableListDTO>>, QueryHandler<GetAllReceivablesQuery, List<ReceivableListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllReceivablesByIdQuery, ReceivableDTO>, QueryHandler<GetAllReceivablesByIdQuery, ReceivableDTO>>();
            services.AddTransient<IRequestHandler<GetPaysByIdQuery, ReceivableDTO>, GetPaysByIdQueryHandler>();
            services.AddTransient<IRequestHandler<CreatePaymentReceiptCommand, CreatePaymentReceiptDTO>, CreateCommandHandler<Entities.Accounts.PaymentReceipt, CreatePaymentReceiptDTO>>();

            return services;
        }
    }
}

