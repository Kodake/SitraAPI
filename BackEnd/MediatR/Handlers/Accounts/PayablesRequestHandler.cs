using BackEnd.DTOs.Accounts.PayableReceipts;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.MediatR.Commands.Accounts.Payables;
using BackEnd.MediatR.Queries.Accounts.Payables;
using MediatR;

namespace BackEnd.MediatR.Handlers.Accounts
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
