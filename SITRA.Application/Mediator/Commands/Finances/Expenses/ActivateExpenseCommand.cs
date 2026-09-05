using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Finances.Expenses
{
    public class ActivateExpenseCommand : ToggleCommand
    {
        public ActivateExpenseCommand(Guid id) : base(id)
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
