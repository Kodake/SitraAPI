using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Finances.Expenses
{
    public class CreateExpenseCommand(CreateExpenseDTO dto) : CreateCommand<Expense, CreateExpenseDTO>(dto)
    {
        public override async Task<CreateExpenseDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var expense = mapper.Map<Expense>(Dto);
            await unitOfWork.ExpenseRepository.CreateAsync(expense);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("expenses-get", cancellationToken);
            return Dto;
        }
    }
}
