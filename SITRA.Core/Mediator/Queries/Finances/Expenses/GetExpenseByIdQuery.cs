using AutoMapper;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Expenses
{
    public class GetExpenseByIdQuery(Guid id) : GetByIdQuery<ExpenseDTO>(id)
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
