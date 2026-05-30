using MediatR;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.Mediator.Commands.CashBoxes;
using SITRA.Core.Mediator.Queries.CashBoxes;

namespace SITRA.Core.Mediator.Handlers.CashBoxes
{
    public static class ContactsRequestHandler
    {
        public static IServiceCollection AddCashBoxServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetPettyCashByUserIdQuery, PettyCashDTO?>, GetPettyCashByUserIdQueryHandler>();
            services.AddTransient<IRequestHandler<CreateOpenPettyCashCommand, CreateOpenPettyCashDTO>, CreateCommandHandler<Entities.CashBoxes.PettyCash, CreateOpenPettyCashDTO>>();
            services.AddTransient<
                IRequestHandler<UpdateClosePettyCashCommand, (byte[] FileBytes, string FileName)?>,
                UpdateClosePettyCashCommandHandler>();
            services.AddTransient<IRequestHandler<GetAllPettyCashMovementsByIdQuery, List<PettyCashMovementListDTO>>, QueryHandler<GetAllPettyCashMovementsByIdQuery, List<PettyCashMovementListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllPaymentMethodSummaryByIdQuery, List<PaymentMethodAmountListDTO>>, GetAllPaymentMethodSummaryByIdQueryHandler>();

            services.AddTransient<IRequestHandler<CreateWithdrawalPettyCashMovementCommand, CreateWithdrawalPettyCashMovementDTO>, CreateWithdrawalPettyCashMovementCommandHandler>();
            services.AddTransient<IRequestHandler<GetAllWithdrawalPettyCashMovementsByIdQuery, List<WithdrawalPettyCashMovementListDTO>>, QueryHandler<GetAllWithdrawalPettyCashMovementsByIdQuery, List<WithdrawalPettyCashMovementListDTO>>>();

            return services;
        }
    }
}

