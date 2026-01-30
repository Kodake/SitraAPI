using MediatR;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Mediator.Commands.Finances.Expenses;
using SITRA.Core.Mediator.Queries.Finances.Expenses;

namespace SITRA.Core.Mediator.Handlers.Finances
{
    public static class ExpensesRequestHandler
    {
        public static IServiceCollection AddExpenseServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateExpenseCommand, bool>, ToggleCommandHandler<ActivateExpenseCommand>>();
            services.AddTransient<IRequestHandler<DeactivateExpenseCommand, bool>, ToggleCommandHandler<DeactivateExpenseCommand>>();
            services.AddTransient<IRequestHandler<GetExpenseByIdQuery, ExpenseDTO>, QueryHandler<GetExpenseByIdQuery, ExpenseDTO>>();
            services.AddTransient<IRequestHandler<GetAllExpensesQuery, List<ExpenseListDTO>>, QueryHandler<GetAllExpensesQuery, List<ExpenseListDTO>>>();
            services.AddTransient<IRequestHandler<CreateExpenseCommand, CreateExpenseDTO>, CreateCommandHandler<Entities.Finances.Expense, CreateExpenseDTO>>();
            services.AddTransient<IRequestHandler<UpdateExpenseCommand, bool>, UpdateCommandHandler<Entities.Finances.Expense, CreateExpenseDTO>>();

            return services;
        }
    }
}

