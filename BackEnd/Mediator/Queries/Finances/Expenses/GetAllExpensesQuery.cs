using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Finances.Expenses;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Finances.Expenses
{
    public class GetAllExpensesQuery(PaginationDTO pagination) : GetAllQuery<ExpenseListDTO>(pagination)
    {
        public override async Task<List<ExpenseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var currencies = await unitOfWork.ExpenseRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ExpenseListDTO>>(currencies);
        }
    }
}
