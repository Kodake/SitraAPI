using Mediator;
using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Mediator.Commands.Accounts.Payables;
using SITRA.Core.Mediator.Queries.Accounts.Payables;

namespace SITRA.Core.Mediator.Handlers.Accounts
{
    public static class PayablesRequestHandler
    {
        public static IServiceCollection AddPayableServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<DeactivatePayableCommand, bool>, ToggleCommandHandler<DeactivatePayableCommand>>();
            services.AddTransient<IRequestHandler<GetAllPayablesQuery, List<PayableListDTO>>, QueryHandler<GetAllPayablesQuery, List<PayableListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllPayablesByIdQuery, PayableDTO>, QueryHandler<GetAllPayablesByIdQuery, PayableDTO>>();
            services.AddTransient<IRequestHandler<GetPaysByIdQuery, PayableDTO>, GetPaysByIdQueryHandler>();
            services.AddTransient<IRequestHandler<CreatePayableReceiptCommand, CreatePayableReceiptDTO>, CreateCommandHandler<Entities.Accounts.PayableReceipt, CreatePayableReceiptDTO>>();

            return services;
        }
    }
}

