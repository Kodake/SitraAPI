using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Expenses
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
