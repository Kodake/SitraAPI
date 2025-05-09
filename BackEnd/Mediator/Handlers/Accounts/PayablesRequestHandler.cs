using BackEnd.DTOs.Accounts.PayableReceipts;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Mediator.Commands.Accounts.Payables;
using BackEnd.Mediator.Queries.Accounts.Payables;
using MediatR;

namespace BackEnd.Mediator.Handlers.Accounts
{
    public static class PayablesRequestHandler
    {
        public static IServiceCollection AddPayableServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<DeactivatePayableCommand, bool>, ToggleCommandHandler<DeactivatePayableCommand>>();
            services.AddTransient<IRequestHandler<GetAllPayablesQuery, List<PayableListDTO>>, QueryHandler<GetAllPayablesQuery, List<PayableListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllPayablesByIdQuery, PayableDTO>, QueryHandler<GetAllPayablesByIdQuery, PayableDTO>>();
            services.AddTransient<IRequestHandler<GetPaysByIdQuery, PayableDTO>, QueryHandler<GetPaysByIdQuery, PayableDTO>>();
            services.AddTransient<IRequestHandler<CreatePayableReceiptCommand, CreatePayableReceiptDTO>, CreateCommandHandler<Entities.Accounts.PayableReceipt, CreatePayableReceiptDTO>>();

            return services;
        }
    }
}
