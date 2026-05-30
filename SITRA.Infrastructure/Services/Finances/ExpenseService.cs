using SITRA.Core.Entities.Finances;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Finances;
using System.Linq.Expressions;

namespace SITRA.Infrastructure.Services.Finances
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Expense>> GetAllAsync(StatusType status, CancellationToken cancellationToken = default)
        {
            Expression<Func<Expense, bool>> filter = status switch
            {
                StatusType.Activated => u => u.IsActive,
                StatusType.Deactivated => u => !u.IsActive,
                _ => u => true
            };

            var items = await _unitOfWork.ExpenseRepository.GetAllAsync(filter, cancellationToken);
            return items.OrderBy(u => u.Id).ToList();
        }
    }
}
