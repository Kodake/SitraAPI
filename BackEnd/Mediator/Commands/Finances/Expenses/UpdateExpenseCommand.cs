using AutoMapper;
using BackEnd.DTOs.Finances.Expenses;
using BackEnd.Entities.Finances;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Finances.Expenses
{
    public class UpdateExpenseCommand(long id, CreateExpenseDTO dto) : UpdateCommand<Expense, CreateExpenseDTO>(id, dto)
    {
        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var expenseDB = await unitOfWork.ExpenseRepository.GetByIdAsync(Id);

            if (expenseDB == null) return false;

            mapper.Map(Dto, expenseDB);
            unitOfWork.ExpenseRepository.Update(expenseDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("expenses-get", default);

            return true;
        }
    }
}
