using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Finances.Expenses
{
    public class ActivateExpenseCommand : ToggleCommand
    {
        public ActivateExpenseCommand(long id) : base(id)
        { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(Id);

            if (expense == null) return false;

            await unitOfWork.ExpenseRepository.ActivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("expenses-get", cancellationToken);

            return true;
        }
    }
}
