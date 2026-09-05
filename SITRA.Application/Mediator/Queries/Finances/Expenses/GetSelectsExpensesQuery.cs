using SITRA.Core.Interfaces.Services.Finances;
using Mediator;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Finances.Expenses
{
    public class GetSelectsExpensesQuery : IRequest<List<ExpenseListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsExpensesQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsExpensesQueryHandler : IRequestHandler<GetSelectsExpensesQuery, List<ExpenseListDTO>>
    {
        private readonly IExpenseService _expenseService;
        private readonly IMapper _mapper;

        public GetSelectsExpensesQueryHandler(IExpenseService expenseService, IMapper mapper)
        {
            _expenseService = expenseService;
            _mapper = mapper;
        }

        public async ValueTask<List<ExpenseListDTO>> Handle(GetSelectsExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<ExpenseListDTO>>(expenses);
        }
    }
}
