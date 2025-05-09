using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.DTOs.Accounts.Receivables;
using BackEnd.Mediator.Commands.Accounts.Receivables;
using BackEnd.Mediator.Queries.Receivables;
using MediatR;

namespace BackEnd.Mediator.Handlers.Accounts
{
    public static class ReceivablesRequestHandler
    {
        public static IServiceCollection AddReceivableServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<DeactivateReceivableCommand, bool>, ToggleCommandHandler<DeactivateReceivableCommand>>();
            services.AddTransient<IRequestHandler<GetAllReceivablesQuery, List<ReceivableListDTO>>, QueryHandler<GetAllReceivablesQuery, List<ReceivableListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllReceivablesByIdQuery, ReceivableDTO>, QueryHandler<GetAllReceivablesByIdQuery, ReceivableDTO>>();
            services.AddTransient<IRequestHandler<GetPaysByIdQuery, ReceivableDTO>, QueryHandler<GetPaysByIdQuery, ReceivableDTO>>();
            services.AddTransient<IRequestHandler<CreatePaymentReceiptCommand, CreatePaymentReceiptDTO>, CreateCommandHandler<Entities.Accounts.PaymentReceipt, CreatePaymentReceiptDTO>>();

            return services;
        }
    }
}
