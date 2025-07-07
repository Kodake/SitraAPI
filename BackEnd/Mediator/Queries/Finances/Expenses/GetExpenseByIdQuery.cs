using AutoMapper;
using BackEnd.DTOs.Finances.Expenses;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Finances.Expenses
{
    public class GetExpenseByIdQuery(long id) : GetByIdQuery<ExpenseDTO>(id)
    {
        public override async Task<ExpenseDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var expense = await unitOfWork.ExpenseRepository.GetByIdAsync(Id);
            return mapper.Map<ExpenseDTO>(expense);
        }
    }
}
