using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Finances.Expenses
{
    public class DeactivateExpenseCommand(Guid id) : ToggleCommand(id)
    {
        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(Id);

            if (expense == null) return false;

            await unitOfWork.ExpenseRepository.DeactivateAsync(Id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("expense-get", cancellationToken);

            return true;
        }
    }
}
