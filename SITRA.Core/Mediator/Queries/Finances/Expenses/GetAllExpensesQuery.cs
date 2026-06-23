using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Interfaces.Services.Finances;

namespace SITRA.Core.Mediator.Queries.Finances.Expenses
{
    public class GetAllExpensesQuery : IRequest<List<ExpenseListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllExpensesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllExpensesQueryHandler : IRequestHandler<GetAllExpensesQuery, List<ExpenseListDTO>>
    {
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;

        public GetAllExpensesQueryHandler(IExpenseService expenseService, IMapper mapper)
        {
            _expenseService = expenseService;
            _mapper = mapper;
        }

        public async ValueTask<List<ExpenseListDTO>> Handle(GetAllExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ExpenseListDTO>>(expenses);
        }
    }
}
