using BackEnd.DTOs.CashBoxes;
using BackEnd.Mediator.Commands.CashBoxes;
using BackEnd.Mediator.Queries.CashBoxes;
using MediatR;

namespace BackEnd.Mediator.Handlers.CashBoxes
{
    public static class ContactsRequestHandler
    {
        public static IServiceCollection AddCashBoxServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetPettyCashByUserIdQuery, PettyCashDTO?>, QueryHandler<GetPettyCashByUserIdQuery, PettyCashDTO?>>();
            services.AddTransient<IRequestHandler<CreateOpenPettyCashCommand, CreateOpenPettyCashDTO>, CreateCommandHandler<Entities.CashBoxes.PettyCash, CreateOpenPettyCashDTO>>();
            services.AddTransient<IRequestHandler<UpdateClosePettyCashCommand, bool>, UpdateCommandHandler<Entities.CashBoxes.PettyCash, CreateClosePettyCashDTO>>();
            services.AddTransient<IRequestHandler<GetAllPettyCashMovementsByIdQuery, List<PettyCashMovementListDTO>>, QueryHandler<GetAllPettyCashMovementsByIdQuery, List<PettyCashMovementListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllPaymentMethodSummaryByIdQuery, List<PaymentMethodAmountListDTO>>, QueryHandler<GetAllPaymentMethodSummaryByIdQuery, List<PaymentMethodAmountListDTO>>>();

            services.AddTransient<IRequestHandler<CreateWithdrawalPettyCashMovementCommand, CreateWithdrawalPettyCashMovementDTO>, CreateCommandHandler<Entities.CashBoxes.PettyCashMovement, CreateWithdrawalPettyCashMovementDTO>>();
            services.AddTransient<IRequestHandler<GetAllWithdrawalPettyCashMovementsByIdQuery, List<WithdrawalPettyCashMovementListDTO>>, QueryHandler<GetAllWithdrawalPettyCashMovementsByIdQuery, List<WithdrawalPettyCashMovementListDTO>>>();

            return services;
        }
    }
}
