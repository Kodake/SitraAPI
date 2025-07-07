using AutoMapper;
using BackEnd.DTOs.Finances.Expenses;
using BackEnd.Entities.Finances;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Finances.Expenses
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
